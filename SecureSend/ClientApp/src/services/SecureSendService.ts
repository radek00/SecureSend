import endpoints from "@/config/endpoints";
import type { CancelSecureUpload } from "@/models/CancelSecureUpload";
import type { SecureUploadDto } from "@/models/SecureUploadDto";
import type { ViewSecureUpload } from "@/models/ViewSecureUpload";
import { fetchWrapper } from "@/utils/fetchWrapper";

export abstract class SecureSendService {
  static createSecureUpload = async (
    uuid: string,
    expiryDate: string
  ): Promise<void> => {
    return await fetchWrapper.post<void>(
      `${endpoints.secureSend}?uploadId=${uuid}&expiryDate=${expiryDate}`
    );
  };

  static uploadChunk = async (
    id: string,
    chunkNumber: number,
    totalChunks: number,
    name: string,
    chunk: ArrayBuffer,
    fileType: string,
    controller?: AbortSignal
  ): Promise<void> => {
    const formData = new FormData();
    formData.append("chunk", new Blob([chunk], { type: fileType }), name);
    chunkNumber = +chunkNumber + 1;
    const requestOptions: RequestInit = {
      method: "POST",
      body: formData,
      signal: controller ?? undefined,
    };
    return await fetchWrapper.post<void>(
      `${endpoints.uploadChunks}?uploadId=${id}&chunkNumber=${chunkNumber}&totalChunks=${totalChunks}`,
      undefined,
      requestOptions
    );
  };

  static viewSecureUpload = async (
    viewSecureUpload: ViewSecureUpload
  ): Promise<SecureUploadDto> => {
    return await fetchWrapper.put<SecureUploadDto>(
      `${endpoints.secureSend}?id=${viewSecureUpload.id}`
    );
  };

  static cancelUpload = async (
    cancelSecureUpload: CancelSecureUpload
  ): Promise<void> => {
    return await fetchWrapper.delete(
      `${endpoints.cancelUpload}?id=${cancelSecureUpload.id}&fileName=${cancelSecureUpload.fileName}`
    );
  };
}
