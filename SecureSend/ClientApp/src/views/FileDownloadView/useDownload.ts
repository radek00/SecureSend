import { SecureSendService } from "@/services/SecureSendService";
import { DownloadState, type DownloadStateTuple } from "@/models/DownloadState";
import type { SecureUploadDto } from "@/models/SecureUploadDto";
import type { FileMetadata } from "@/models/SecureFileDto";
import { onUnmounted, ref } from "vue";
import type { IWorkerInit } from "@/models/WorkerInit";
import AuthenticatedSecretKeyCryptographyService from "@/utils/AuthenticatedSecretKeyCryptographyService";

export function useDownloadAll() {
  const broadcast = new BroadcastChannel("progress-channel");

  const fileDownloadStatuses = ref<Map<string, DownloadStateTuple>>(
    new Map<string, DownloadStateTuple>()
  );
  
  const fileMetadata = new Map<string, FileMetadata>();

  const setupDownload = async (
    secureUpload: SecureUploadDto,
    b64Key: string,
    password?: string
  ) => {
    // Initialize crypto service to decrypt metadata
    const keychain = new AuthenticatedSecretKeyCryptographyService(
      password,
      b64Key
    );
    await keychain.start();
    
    // Decrypt all file metadata
    if (secureUpload.files) {
      for (const file of secureUpload.files) {
        const metadata = await keychain.decryptMetadata(file.metadata) as FileMetadata;
        fileMetadata.set(file.fileName, metadata);
        fileDownloadStatuses.value.set(file.fileName, [
          "Download not started",
          DownloadState.NewFile,
        ]);
      }
    }
    //todo
    //check reactivity of fileMetadata
    //upgrade db containers => set concrete versions in docker compose
    //adjust tests
    //add end-to-end tests with playwright
    
    navigator.serviceWorker.controller?.postMessage({
      request: "init",
      id: secureUpload.secureUploadId,
      b64key: b64Key,
      password: password ?? undefined,
      metadata: fileMetadata,
    } as IWorkerInit);
  };
  const downloadAll = async (secureUpload: SecureUploadDto) => {
    const directoryHandle = await window.showDirectoryPicker();
    const files = secureUpload.files!;
    const promises = [];
    for (const file of files) {
      const promise = async (): Promise<void> => {
        try {
          const metadata = fileMetadata.get(file.fileName);
          if (!metadata) {
            throw new Error("Metadata not found");
          }
          
          const writableStream = await (
            await directoryHandle.getFileHandle(metadata.fileName, {
              create: true,
            })
          ).createWritable();
          const response = await SecureSendService.downloadFile(
            secureUpload.secureUploadId!,
            file.fileName
          );
          await response.body!.pipeTo(writableStream);
          fileDownloadStatuses.value.set(file.fileName, [
            "Download completed",
            DownloadState.Completed,
          ]);
        } catch {
          fileDownloadStatuses.value.set(file.fileName, [
            "Error with downloading file",
            DownloadState.Failed,
          ]);
        }
      };
      promises.push(promise());
    }
    await Promise.all(promises);
  };

  const onProgressUpdate = (event: any) => {
    if (event.data.request === "progress") {
      const stateTuple: DownloadStateTuple =
        event.data.value === "100%"
          ? ["Download completed", DownloadState.Completed]
          : [event.data.value, DownloadState.InProgress];
      fileDownloadStatuses.value.set(event.data.fileName, stateTuple);
    }
  };
  broadcast.addEventListener("message", onProgressUpdate);

  onUnmounted(() => {
    broadcast.removeEventListener("message", onProgressUpdate);
    broadcast.close();
  });

  return { downloadAll, setupDownload, fileDownloadStatuses, fileMetadata };
}
