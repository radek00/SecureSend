namespace SecureSend.Domain.ReadModels
{
    public class UploadedFilesReadModel
    {
        public int Id { get; set; }
        public string DisplayFileName { get; set; }

        public string RandomFileName { get; set; }
        public string ContentType { get; set; }
        public Guid SecureSendUploadId { get; set; }
        public SecureUploadsReadModel SecureUpload { get; set; }
    }
}