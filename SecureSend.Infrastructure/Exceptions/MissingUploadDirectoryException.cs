using SecureSend.Domain.Exceptions;

namespace SecureSend.Infrastructure.Exceptions;

public class MissingUploadDirectoryException: SecureSendException
{
    public MissingUploadDirectoryException(Guid uploadId) : base($"Missing directory for upload: {uploadId}. Upload has to be setup first.")
    {
    }
}