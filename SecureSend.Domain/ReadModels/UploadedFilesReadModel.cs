namespace SecureSend.Domain.ReadModels
{
    public class UploadedFilesReadModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public Guid SecureSendUploadId { get; set; }
        public SecureUploadsReadModel SecureUpload { get; set; }
    }
}