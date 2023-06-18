import endpoints from "@/config/endpoints";
import type { SecureUploadDto } from "@/models/SecureUploadDto";
import type { ViewSecureUpload } from "@/models/ViewSecureUpload";
import { fetchWrapper } from "@/utils/fetchWrapper";

export abstract class SecureSendService {
    static createSecureUpload = async (uuid: string): Promise<void> => {
        return await fetchWrapper.post<void>(`${endpoints.secureSend}?uploadId=${uuid}`)
    }

    static uploadChunk = async(id: string, chunkNumber: number, totalChunks: number, name: string, chunk: ArrayBuffer): Promise<void> => {
        const formData = new FormData();
        formData.append('chunk', new Blob([chunk]), name);
        chunkNumber = +chunkNumber + 1;
        const requestOptions = {
            method: 'POST',
            body: formData
        }
        return await fetchWrapper.post<void>(`${endpoints.uploadChunks}?uploadId=${id}&chunkNumber=${chunkNumber}&totalChunks=${totalChunks}`, undefined, requestOptions);
    }

    static viewSecureUpload = async (viewSecureUpload: ViewSecureUpload): Promise<SecureUploadDto> => {
        return await fetchWrapper.put(`${endpoints.secureSend}?id=${viewSecureUpload.id}`);
    }
}