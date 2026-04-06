using SecureSend.Domain.Exceptions;

namespace SecureSend.Domain.ValueObjects
{
    public record SecureSendFile
    {
        public string FileName { get; }
        public string Metadata { get; }

        public SecureSendFile(string fileName, string metadata)
        {
            FileName = fileName;
            Metadata = metadata;
        }

        public static SecureSendFile Create(string metadata)
        {
            if (string.IsNullOrEmpty(metadata)) 
                throw new MissingMetadataException("Metadata must be provided.");
            
            return new SecureSendFile(Path.GetRandomFileName(), metadata);
        }
    }
}
