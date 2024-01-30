using SecureSend.Domain.Exceptions;

namespace SecureSend.Application.Exceptions;

public class SizeLimitExceededException: SecureSendException
{
    public SizeLimitExceededException() : base("Upload size limit exceeded")
    {
    }
}