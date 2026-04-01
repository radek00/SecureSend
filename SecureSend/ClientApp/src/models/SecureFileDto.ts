export interface SecureFileDto {
  fileName: string;
  metadata: string;
}

export interface FileMetadata {
  fileName: string;
  contentType: string;
  fileSize: number;
}
