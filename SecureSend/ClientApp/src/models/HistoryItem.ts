import type { SecureFileDto } from "@/models/SecureFileDto";

export interface HistoryItem {
  link: string;
  uploadDate: string;
  expirationDate: string;
  files: Partial<SecureFileDto>[];
}
