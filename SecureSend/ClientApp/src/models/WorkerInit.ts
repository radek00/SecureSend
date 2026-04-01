import type { FileMetadata } from "./SecureFileDto";

export interface IWorkerInit {
  request: string;
  b64key: string;
  password?: string;
  id: string;
  metadata?: Record<string, FileMetadata>;
}
