using SecureSend.Domain.Exceptions;

namespace SecureSend.Application.Exceptions
{
    public class UploadDoesNotExistException : SecureSendNotFoundException
    {
        public UploadDoesNotExistException(Guid id) : base($"Upload with id: {id} does not exist.")
        {
        }
    }
}
