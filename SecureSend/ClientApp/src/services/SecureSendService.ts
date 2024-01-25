import endpoints from "@/config/endpoints";
import type { CancelSecureUpload } from "@/models/CancelSecureUpload";
import type { SecureUploadDto } from "@/models/SecureUploadDto";
import type { ViewSecureUpload } from "@/models/ViewSecureUpload";
import { fetchWrapper } from "@/utils/fetchWrapper";
import type { CreateSecureUpload } from "@/models/CreateSecureUpload";
import type { UploadVerifyResponseDTO } from "@/models/VerifyUploadResponseDTO";

export abstract class SecureSendService {
  static createSecureUpload = async (
    secureUpload: CreateSecureUpload
  ): Promise<void> => {
    return await fetchWrapper.post<void, CreateSecureUpload>(
      endpoints.secureSend,
      secureUpload
    );
  };

  static uploadChunk = async (
    id: string,
    chunkNumber: number,
    totalChunks: number,
    name: string,
    chunk: ArrayBuffer,
    fileType: string,
    totalFileSize: number,
    chunkId: string,
    controller?: AbortSignal
  ): Promise<void> => {
    const formData = new FormData();
    formData.append("chunk", new Blob([chunk], { type: fileType }), name);
    const requestOptions: RequestInit = {
      method: "POST",
      body: formData,
      signal: controller ?? undefined,
    };
    return await fetchWrapper.post<void>(
      `${endpoints.uploadChunks}?uploadId=${id}&chunkNumber=${chunkNumber}&totalChunks=${totalChunks}&totalFileSize=${totalFileSize}&chunkId=${chunkId}`,
      undefined,
      requestOptions
    );
  };

  static viewSecureUpload = async (
    viewSecureUpload: ViewSecureUpload
  ): Promise<SecureUploadDto> => {
    return await fetchWrapper.put<SecureUploadDto, ViewSecureUpload>(
      `${endpoints.secureSend}`,
      viewSecureUpload
    );
  };

  static cancelUpload = async (
    cancelSecureUpload: CancelSecureUpload
  ): Promise<void> => {
    return await fetchWrapper.delete(
      `${endpoints.cancelUpload}?id=${cancelSecureUpload.id}&fileName=${cancelSecureUpload.fileName}`
    );
  };

  static verifySecureUpload = async (
    id: string
  ): Promise<UploadVerifyResponseDTO> => {
    return await fetchWrapper.get(`${endpoints.secureSend}?id=${id}`);
  };

  static downloadFile = async (id: string, fileName: string): Promise<Response> => {
    const response = await fetch(`${endpoints.download}?id=${
      id
    }&fileName=${fileName}`);
    if (!response.ok) throw new Error(response.statusText);
    return response;
  }
}
