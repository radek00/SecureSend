namespace SecureSend.Domain.ReadModels
{
    public class SecureUploadsReadModel
    {
        public Guid Id { get; init; }
        public DateTime? ExpiryDate { get; init; }
        public required bool IsViewed { get; init; }
        public required DateTime UploadDate { get; init; }
        public byte[]? PasswordHash { get; init; }
        public ICollection<UploadedFilesReadModel>? Files { get; init; }
    }
}
