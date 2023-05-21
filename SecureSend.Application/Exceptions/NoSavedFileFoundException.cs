using SecureSend.Domain.Exceptions;

namespace SecureSend.Application.Exceptions
{
    public class NoSavedFileFoundException : SecureSendException
    {
        public NoSavedFileFoundException(string name, Guid dir) : base($"File with name: {name} does not exist in upload directory: {dir}.")
        {
        }
    }
}
