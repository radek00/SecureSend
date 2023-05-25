namespace SecureSend.Domain.Exceptions
{
    public class SecureSendNotFoundException : SecureSendException
    {
        public SecureSendNotFoundException(string message) : base(message)
        {
        }
    }
}
