using SecureSend.Domain.Exceptions;
using SecureSend.Domain.ValueObjects;


namespace SecureSend.Domain.Entities
{
    public class SecureSendUpload
    {
        public SecureSendUploadId Id { get; private set; } = null!;
        public SecureSendUploadDate UploadDate { get; private set; } = null!;
        public SecureSendExpiryDate? ExpiryDate { get; private set; }
        public SecureSendIsViewed IsViewed { get; private set; } = null!;
        public SecureSendPasswordHash? PasswordHash { get; private set; }
        
        public List<SecureSendFile> Files { get; private set; } = new();

    public SecureSendUpload(SecureSendUploadId id, SecureSendUploadDate uploadDate, SecureSendExpiryDate? expiryDate, SecureSendIsViewed isViewedl, SecureSendPasswordHash? passwordHash)
        {
            Id = id;
            UploadDate = uploadDate;
            ExpiryDate = expiryDate;
            IsViewed = isViewedl;
            PasswordHash = passwordHash;
        }

        public SecureSendUpload()
        {
        }

        public void AddFile(SecureSendFile file)
        {
            var alreadeExists = Files.Any(f => f.DisplayFileName == file.DisplayFileName);
            if (alreadeExists) throw new FileAlreadyExistsException(Id, file.DisplayFileName);
            Files.Add(file);
        }

        public void RemoveFile(string fileName)
        {
            var existingFile = Files.FirstOrDefault(f => f.DisplayFileName == fileName);
            if (existingFile is not null) Files.Remove(existingFile);
        }

        public void AddMultipleFiles(IEnumerable<SecureSendFile> files)
        {
            foreach (var file in files) AddFile(file);
        }

        public void MarkAsViewed()
        {
            IsViewed = true;
        }


    }
}
