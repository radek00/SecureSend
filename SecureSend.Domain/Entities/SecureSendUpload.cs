using SecureSend.Domain.Exceptions;
using SecureSend.Domain.ValueObjects;


namespace SecureSend.Domain.Entities
{
    public class SecureSendUpload
    {
        public SecureSendUploadId Id {get; private set;}
        private SecureSendUploadDate _uploadDate;
        private SecureSendExpiryDate _expiryDate;
        private SecureSendIsViewed _isViewedl;
        private List<SecureSendFile> _files = new();

        public SecureSendUpload(SecureSendUploadId id, SecureSendUploadDate uploadDate, SecureSendExpiryDate expiryDate, SecureSendIsViewed isViewedl)
        {
            Id = id;
            _uploadDate = uploadDate;
            _expiryDate = expiryDate;
            _isViewedl = isViewedl;
        }

        public SecureSendUpload()
        {
        }

        public void AddFile(SecureSendFile file)
        {
            var alreadeExists = _files.Any(f => f.FileName == file.FileName);
            if (alreadeExists) throw new FileAlreadyExistsException(Id, file.FileName);
            _files.Add(file);
        }

        public void AddMultipleFiles(IEnumerable<SecureSendFile> files)
        {
            foreach (var file in files) AddFile(file);
        }


    }
}
