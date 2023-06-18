﻿namespace SecureSend.Domain.ReadModels
{
    public class SecureUploadsReadModel
    {
        public Guid Id { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsViewed { get; set; }
        public DateTime UploadDate { get; set; }
        public ICollection<UploadedFilesReadModel> Files { get; set; }
    }
}