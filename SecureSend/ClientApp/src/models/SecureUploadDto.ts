import type { SecureFileDto } from "./SecureFileDto";

export interface SecureUploadDto {
  secureUploadId: string;
  uploadDate: string;
  expiryDate: string | null;
  files: SecureFileDto[] | null;
}
