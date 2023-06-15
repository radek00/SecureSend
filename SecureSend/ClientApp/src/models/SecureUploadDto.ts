import type { SecureFileDto } from "./SecureFileDto";

export interface SecureUploadDto {
    SecureUploadId: string;
    UploadDate: string;
    ExpiryDate: string | null;
    Files: SecureFileDto[] | null;
}