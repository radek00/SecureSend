import {SecureSendService} from "@/services/SecureSendService";
import AuthenticatedSecretKeyCryptographyService from "@/utils/AuthenticatedSecretKeyCryptographyService";
import type {IMappedFormValues} from "@/views/FileUploadView/useFileUploadForm";
import type {UploadStateTuple} from "@/models/UploadStateTuple";
import {UploadState} from "@/models/UploadStateTuple";
import {inject, ref} from "vue";
import type { Ref } from "vue"
import splitFile from "@/utils/splitFile";


export enum UploadResult {
    Partial,
    AllCanceled,
    Failed,
    Success
}
export function useUpload(values: IMappedFormValues) {
    const isLoading = inject<Ref<boolean>>("isLoading");
    const isUploadSetup = ref<boolean>(false);
    let keychain: AuthenticatedSecretKeyCryptographyService;
    let uuid = self.crypto.randomUUID();

    const files = ref(new Map<File, UploadStateTuple>());
    const fileKeys = new Map<string, boolean>();
    const pausedFiles = new Map<File, boolean | ((value?: string) => void)>();
    const controllers = new Map<string, AbortController>();
    async function setupUpload (){
        await SecureSendService.createSecureUpload({
            uploadId: uuid,
            expiryDate: values.expiryDate ? values.expiryDate : null,
            password: values.password,
        });
        keychain = new AuthenticatedSecretKeyCryptographyService(
            values.password ? values.password : undefined
        );
        await keychain.start();
    }

    async function handleUploadCallback(
        chunk: ArrayBuffer,
        num: number,
        totalChunks: number,
        file: File,
        chunkId: string
    ) {
        try {
            await handlePause(file);
            const currentChunk = num + 1;
            await SecureSendService.uploadChunk(
                uuid,
                currentChunk,
                totalChunks,
                file.name,
                chunk,
                file.type,
                file.size,
                chunkId,
                controllers.get(file.name)?.signal
            );
            const progress = Math.ceil((currentChunk / totalChunks) * 100);
            const stateTuple: UploadStateTuple =
                currentChunk === totalChunks
                    ? ["Upload completed", UploadState.Completed]
                    : currentChunk === totalChunks - 1
                        ? ["Finishing upload...", UploadState.Merging]
                        : [`${progress}%`, UploadState.InProgress];
            files.value.set(file, stateTuple);
        } catch (error: any) {
            if (
                error === UploadState.Cancelled ||
                error.code === DOMException.ABORT_ERR
            ) {
                files.value.set(file, ["Upload cancelled.", UploadState.Cancelled]);
            } else {
                files.value.set(file, ["Error with uploading file.", UploadState.Failed]);
            }
            throw error;
        }
    }

    async function handlePause(file: File) {
        if (!controllers.has(file.name)) {
            const controller = new AbortController();
            controller.signal.addEventListener("pause", pauseEventListener);
            controllers.set(file.name, controller);
        }
        if (pausedFiles.get(file)) {
            files.value.set(file, ["Upload paused", UploadState.Paused]);
            const promise = new Promise((res) => {
                pausedFiles.set(file, res);
            });
            await promise;
        }
    }

    const onCancel = async (fileObj: File) => {
        const controller = controllers.get(fileObj.name);
        if (controller) {
            controller.abort(UploadState.Cancelled);
            onResume(fileObj);
            await SecureSendService.cancelUpload({ id: uuid, fileName: fileObj.name });
        }
    };

    const onPause = (fileObj: File) => {
        const controller = controllers.get(fileObj.name);
        controller?.signal.dispatchEvent(
            new CustomEvent("pause", { detail: { file: fileObj } })
        );
    };

    const onResume = (fileObj: File) => {
        const resolve = pausedFiles.get(fileObj);
        if (resolve && typeof resolve == "function") {
            resolve();
            pausedFiles.set(fileObj, false);
        }
    };
    const onFilesChange = (formFiles: FileList | undefined | null) => {
        if (formFiles) {
            for (let i = 0; i < formFiles.length; i++) {
                const file = formFiles[i];
                if (!fileKeys.has(file.name)) {
                    files.value.set(file, [`Upload not started`, UploadState.NewFile]);
                    fileKeys.set(file.name, true);
                }
            }
        }
    };
    
    const onFileRemove = (file: File) => {
        files.value.delete(file);
        fileKeys.delete(file.name);
    }

    const pauseEventListener = (event: any) => {
        pausedFiles.set(event.detail.file, true);
    };

    const createDownloadUrl = () => {
        return window.location.origin
            .toString()
            .concat(`/download/${uuid}#${keychain.getSecret()}`);
    };

    async function uploadFiles() {
        const requests: Promise<unknown>[] = [];
        for (const [file] of files.value) {
            const chunkId = self.crypto.randomUUID();
            const promise = splitFile(
                file,
                5 * 1024 * 1024,
                async (chunk: ArrayBuffer, num, totalChunks) => {
                    await handleUploadCallback(chunk, num, totalChunks, file, chunkId);
                },
                async (chunk, num) => await keychain.encrypt(chunk, num)
            );
            requests.push(promise);
        }
        const results = await Promise.allSettled(requests);
        if (
            results.find(
                (promise) =>
                    promise.status === "rejected" &&
                    promise.reason.code !== DOMException.ABORT_ERR &&
                    promise.reason !== UploadState.Cancelled
            )
        ) {
            throw new Error("Upload error");
        }
    }

    const handleUpload = async() => {
        isLoading!.value = true;
        if (!isUploadSetup.value) {
            await setupUpload();
            isUploadSetup.value = true;
        }
        try {
            await uploadFiles();
            isLoading!.value = false;
            if (
                [...files.value.values()].find((file) => file[1] === UploadState.Completed)
            ) {
                return UploadResult.Success
            } else {
                fileKeys.clear();
                files.value.clear();
                controllers.clear();
                return UploadResult.AllCanceled
            }
        } catch (error) {
            //fail due to Upload Error
            if (
                ![...files.value.values()].find(
                    (file) => file[1] === UploadState.Completed
                )
            ) {
                //No files were uploaded
                return UploadResult.Failed
            } else {
                //at least one was successful
                return UploadResult.Partial
            }
        }
    }
    
    const resetUpload = () => {
        uuid = self.crypto.randomUUID();
        files.value.clear();
        fileKeys.clear();
        [...controllers.values()].forEach((con) =>
            con.signal.removeEventListener("pause", pauseEventListener)
        );
        controllers.clear();
        pausedFiles.clear();
        isUploadSetup.value = false;
        isLoading!.value = false;
    }
    
    return {resetUpload, handleUpload, onFileRemove, onFilesChange, onCancel, onPause, onResume, createDownloadUrl, isUploadSetup, files}
    
}