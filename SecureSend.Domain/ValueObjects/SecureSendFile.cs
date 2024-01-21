using Microsoft.AspNetCore.StaticFiles;
using SecureSend.Domain.Exceptions;
using System.Net;

namespace SecureSend.Domain.ValueObjects
{
    public record SecureSendFile
    {
        public string DisplayFileName { get; }
        public string RandomFileName { get; }
        public string ContentType { get; }
        public long FileSize { get; }

        public SecureSendFile(string displayFileName, string contentType, long fileSize)
        {
            if (string.IsNullOrEmpty(displayFileName)) throw new EmptyFileNameException();
            DisplayFileName = WebUtility.HtmlEncode(displayFileName);
            ContentType = contentType;
            FileSize = fileSize;
            RandomFileName = Path.GetRandomFileName();
        }
    }
}
