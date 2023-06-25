using SecureSend.Domain.Exceptions;
using SecureSend.Domain.ValueObjects;


namespace SecureSend.Domain.Entities
{
    public class SecureSendUpload
    {
        public SecureSendUploadId Id {get; private set;}
        public SecureSendUploadDate UploadDate { get; private set; }
        public SecureSendExpiryDate ExpiryDate { get; private set; }
        public SecureSendIsViewed IsViewed { get; private set; }

        public SecureSendPasswordHash PasswordHash { get; private set; }
        public List<SecureSendFile> Files { get; private set; } = new();

        public SecureSendUpload(SecureSendUploadId id,
                            SecureSendUploadDate uploadDate,
                            SecureSendExpiryDate expiryDate,
                            SecureSendIsViewed isViewedl, 
                            SecureSendPasswordHash password)
        {
            Id = id;
            UploadDate = uploadDate;
            ExpiryDate = expiryDate;
            IsViewed = isViewedl;
            PasswordHash = password;
        }

        public SecureSendUpload()
        {
        }

        public void AddFile(SecureSendFile file)
        {
            var alreadeExists = Files.Any(f => f.FileName == file.FileName);
            if (alreadeExists) throw new FileAlreadyExistsException(Id, file.FileName);
            Files.Add(file);
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
