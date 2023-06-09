namespace SecureSend.Infrastructure.EF.Models
{
    internal sealed class SecureUploadsReadModel
    {
        public Guid Id { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsViewed { get; set; }
        public DateTime UploadDate { get; set; }
        public ICollection<UploadedFilesReadModel> Files { get; set; }
    }
}
