using Microsoft.AspNetCore.StaticFiles;
using SecureSend.Domain.Exceptions;

namespace SecureSend.Domain.ValueObjects
{
    public record SecureSendFile
    {
        public string FileName { get; }
        public string ContentType { get; }

        public SecureSendFile(string fileName, string contentType)
        {
            if (string.IsNullOrEmpty(fileName)) throw new EmptyFileNameException();
            FileName = fileName;
            ContentType = contentType;
        }
    }
}
