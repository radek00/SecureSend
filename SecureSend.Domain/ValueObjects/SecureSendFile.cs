using SecureSend.Domain.Exceptions;
using System.Net;

namespace SecureSend.Domain.ValueObjects
{
    public record SecureSendFile
    {
        public string RandomFileName { get; }
        public string Metadata { get; }

        public SecureSendFile(string fileName, string metadata)
        {
            RandomFileName = fileName;
            Metadata = metadata;
        }

        public static SecureSendFile Create(string metadata)
        {
            if (string.IsNullOrEmpty(metadata)) 
                throw new ArgumentException("Metadata cannot be null or empty", nameof(metadata));
            
            return new SecureSendFile(Path.GetRandomFileName(), metadata);
        }
    }
}
