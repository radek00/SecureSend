import type { SecureFileDto } from "@/models/SecureFileDto";

export interface HistoryItem {
  link: string;
  uploadDate: Date;
  files: Partial<SecureFileDto>[];
}
