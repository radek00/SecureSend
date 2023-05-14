namespace SecureSend.Domain.Exceptions
{
    public class FileAlreadyExistsException : SecureSendException
    {
        public FileAlreadyExistsException(Guid id, string name) : base($"File name: {name} already exists in upload with id: {id}")
        {
        }
    }
}
