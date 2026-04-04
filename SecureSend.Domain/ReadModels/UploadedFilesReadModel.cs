namespace SecureSend.Domain.ReadModels
{
    public class UploadedFilesReadModel
    {
        public int Id { get; init; }
        public required string RandomFileName { get; init; }
        public required string Metadata { get; init; }
        public required Guid SecureSendUploadId { get; init; }
        public required SecureUploadsReadModel SecureUpload { get; init; }
    }
}