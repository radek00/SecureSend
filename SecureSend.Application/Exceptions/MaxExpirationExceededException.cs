using SecureSend.Domain.Exceptions;

namespace SecureSend.Application.Exceptions;

public class MaxExpirationExceededException : SecureSendException
{
    public MaxExpirationExceededException(int maxExpiration) : base($"Max expiration exceeded. Max allowed expiration is: {maxExpiration}")
    {
    }
}