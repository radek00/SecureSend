namespace SecureSend.Domain.ReadModels
{
    public class UploadedFilesReadModel
    {
        public int Id { get; init; }
        public required string DisplayFileName { get; init; }

        public required string RandomFileName { get; init; }
        public required string ContentType { get; init; }
        public required Guid SecureSendUploadId { get; init; }
        public required SecureUploadsReadModel SecureUpload { get; init; }
    }
}