namespace SecureSend.Application.DTO
{
    public class SecureUploadDto
    {
        public Guid SecureUploadId { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public IEnumerable<SecureFileDto>? Files { get; set; }
    }

    public class SecureFileDto
    {
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public long FileSize { get; set; }
    }
}
