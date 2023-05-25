using SecureSend.Domain.Exceptions;

namespace SecureSend.Application.Exceptions
{
    public class UploadExpiredException : SecureSendException
    {
        public UploadExpiredException(DateTime? expiryDate) : base($"Upload has expired on: {expiryDate}")
        {
        }
    }
}
