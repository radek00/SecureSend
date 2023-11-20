export interface CreateSecureUpload {
  uploadId: string;
  expiryDate: string | null;
  password: string | null;
}
