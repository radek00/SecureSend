import endpoints from "@/config/endpoints";
import { fetchWrapper } from "@/utils/fetchWrapper";

export abstract class SecureSendService {
    static createSecureUpload = async (uuid: string) => {
        return await fetchWrapper.post(`${endpoints.secureSend}?uploadId=${uuid}`)
    }

    static uploadChunk = async(id: string, chunkNumber: number, totalChunks: number, name: string, chunk: ArrayBuffer) => {
        const formData = new FormData();
        formData.append('chunk', new Blob([chunk]), name);
        chunkNumber = +chunkNumber + 1;
        const requestOptions = {
            method: 'POST',
            body: formData
        }
        return await fetchWrapper.post(`${endpoints.uploadChunks}?uploadId=${id}&chunkNumber=${chunkNumber}&totalChunks=${totalChunks}`, undefined, requestOptions);
    }
}