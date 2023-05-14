namespace SecureSend.Domain.Exceptions
{
    public class EmptyFileNameException: SecureSendException
    {
        public EmptyFileNameException(): base("File name cannot be empty")
        {
            
        }
    }
}
