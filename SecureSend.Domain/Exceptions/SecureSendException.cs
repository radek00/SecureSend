namespace SecureSend.Domain.Exceptions
{
    public abstract class SecureSendException: Exception
    {
        protected SecureSendException(string message): base(message) { }
    }
}
