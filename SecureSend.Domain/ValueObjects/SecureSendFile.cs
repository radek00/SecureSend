using Microsoft.AspNetCore.StaticFiles;
using SecureSend.Domain.Exceptions;

namespace SecureSend.Domain.ValueObjects
{
    public record SecureSendFile
    {
        public string DisplayFileName { get; }
        public string RandomFileName { get; set; }
        public string ContentType { get; }
        public long FileSize { get; set; }

        public SecureSendFile(string displayFileName, string contentType, long fileSize, string randomFileName)
        {
            if (string.IsNullOrEmpty(displayFileName)) throw new EmptyFileNameException();
            DisplayFileName = displayFileName;
            ContentType = contentType;
            FileSize = fileSize;
            RandomFileName = randomFileName;
        }
    }
}
