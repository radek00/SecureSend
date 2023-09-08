import type { encryptionKey } from "@/models/utilityTypes/encryptionKey";

export interface IWorkerInit {
  request: string;
  salt: Uint8Array;
  masterKey: encryptionKey;
  id: string;
}
