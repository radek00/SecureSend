using SecureSend.Domain.Exceptions;
using SecureSend.Domain.ValueObjects;


namespace SecureSend.Domain.Entities
{
    public class SecureSendUpload
    {
        public SecureSendUploadId Id { get; private set; } = null!;
        public SecureSendUploadDate UploadDate { get; private set; } = null!;
        public SecureSendExpiryDate? ExpiryDate { get; private set; }
        public SecureSendPasswordHash? PasswordHash { get; private set; }
        
        public List<SecureSendFile> Files { get; private set; } = new();

    public SecureSendUpload(SecureSendUploadId id, SecureSendUploadDate uploadDate, SecureSendExpiryDate? expiryDate, SecureSendPasswordHash? passwordHash)
        {
            Id = id;
            UploadDate = uploadDate;
            ExpiryDate = expiryDate;
            PasswordHash = passwordHash;
        }

        public SecureSendUpload()
        {
        }

        public void AddFile(SecureSendFile file)
        {
            var alreadyExists = Files.Any(f => f.RandomFileName == file.RandomFileName);
            if (alreadyExists) throw new FileAlreadyExistsException(Id, file.RandomFileName);
            Files.Add(file);
        }

        public void RemoveFile(string randomFileName)
        {
            var existingFile = Files.FirstOrDefault(f => f.RandomFileName == randomFileName);
            if (existingFile is not null) Files.Remove(existingFile);
        }

        public void AddMultipleFiles(IEnumerable<SecureSendFile> files)
        {
            foreach (var file in files) AddFile(file);
        }
    }
}
