using SecureSend.Domain.Exceptions;

namespace SecureSend.Domain.Entities;

public class InvalidPasswordException : SecureSendException
{
    public InvalidPasswordException() : base("Provided password is invalid.")
    {
    }
}