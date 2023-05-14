namespace SecureSend.Domain.Exceptions
{
    public class EmptySecureSendUploadIdException : SecureSendException
    {
        public EmptySecureSendUploadIdException() : base("Id cannot be empty")
        {
        }
    }
}
