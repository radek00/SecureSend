import { SecureSendService } from "@/services/SecureSendService";
import { DownloadState, type DownloadStateTuple } from "@/models/DownloadState";
import type { SecureUploadDto } from "@/models/SecureUploadDto";
import { onUnmounted, ref } from "vue";
import type { IWorkerInit } from "@/models/WorkerInit";

export function useDownloadAll() {
  const broadcast = new BroadcastChannel("progress-channel");

  const fileDownloadStatuses = ref<Map<string, DownloadStateTuple>>(
    new Map<string, DownloadStateTuple>()
  );

  const setupDownload = (
    secureUpload: SecureUploadDto,
    b64Key: string,
    password?: string
  ) => {
    secureUpload.files!.forEach((file) =>
      fileDownloadStatuses.value.set(file.fileName!, [
        "Download not started",
        DownloadState.NewFile,
      ])
    );
    navigator.serviceWorker.controller?.postMessage({
      request: "init",
      id: secureUpload.secureUploadId,
      b64key: b64Key,
      password: password,
    } as IWorkerInit);
  };
  const downloadAll = async (secureUpload: SecureUploadDto) => {
    const directoryHandle = await window.showDirectoryPicker();
    const files = secureUpload.files!;
    const promises = [];
    for (const file of files) {
      const promise = async (): Promise<void> => {
        try {
          const writableStream = await (
            await directoryHandle.getFileHandle(file.fileName!, {
              create: true,
            })
          ).createWritable();
          const response = await SecureSendService.downloadFile(
            secureUpload.secureUploadId!,
            file.fileName!
          );
          await response.body!.pipeTo(writableStream);
          fileDownloadStatuses.value.set(file.fileName!, [
            "Download completed",
            DownloadState.Completed,
          ]);
        } catch {
          fileDownloadStatuses.value.set(file.fileName!, [
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

  return { downloadAll, setupDownload, fileDownloadStatuses };
}
