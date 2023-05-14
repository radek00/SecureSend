using Microsoft.AspNetCore.StaticFiles;
using SecureSend.Domain.Exceptions;

namespace SecureSend.Domain.ValueObjects
{
    public record SecureSendFile
    {
        public string FileName { get; }
        public string ContentType { get; }

        public SecureSendFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) throw new EmptyFileNameException();
            FileName = fileName;
            ContentType = GetContentType(fileName);
        }

        private string GetContentType(string fileName)
        {
            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out contentType);
            return contentType ?? "application/octet-stream";
        }
    }
}
