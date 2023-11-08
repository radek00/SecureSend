using Microsoft.AspNetCore.StaticFiles;
using SecureSend.Domain.Exceptions;

namespace SecureSend.Domain.ValueObjects
{
    public record SecureSendFile
    {
        public string FileName { get; }
        public string ContentType { get; }
        public long FileSize { get; set; }

        public SecureSendFile(string fileName, string contentType, long fileSize)
        {
            if (string.IsNullOrEmpty(fileName)) throw new EmptyFileNameException();
            FileName = fileName;
            ContentType = contentType;
            FileSize = fileSize;
        }
    }
}
