namespace SecureSend.Infrastructure.EF.Models
{
    internal sealed class UploadedFilesReadModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public Guid SecureSendUploadId { get; set; }
        public SecureUploadsReadModel SecureUpload { get; set; }
    }
}