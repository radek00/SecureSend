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

        public SecureSendFile(string displayFileName, string contentType, long fileSize, string randomFileName)
        {
            DisplayFileName = displayFileName;
            ContentType = contentType;
            FileSize = fileSize;
            RandomFileName = randomFileName;
        }

        public static SecureSendFile Create(string displayFileName, string contentType, long fileSize)
        {
            if (string.IsNullOrEmpty(displayFileName)) throw new EmptyFileNameException();
            return new SecureSendFile(WebUtility.HtmlEncode(displayFileName), contentType, fileSize,
                Path.GetRandomFileName());
        }
    }
}
