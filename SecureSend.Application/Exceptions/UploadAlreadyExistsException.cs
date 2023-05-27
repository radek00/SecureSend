using SecureSend.Domain.Exceptions;

namespace SecureSend.Application.Exceptions
{
    public class UploadAlreadyExistsException : SecureSendNotFoundException
    {
        public UploadAlreadyExistsException(Guid id) : base($"Upload with id: {id} already exists.")
        {
        }
    }
}
