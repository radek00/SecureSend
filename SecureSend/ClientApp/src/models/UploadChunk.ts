export interface UploadChunk {
    uploadId: Uint8Array;
    chunkNumber: number;
    totalChunks: number;
    chunk: File;
}