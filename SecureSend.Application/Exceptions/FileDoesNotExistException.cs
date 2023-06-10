using SecureSend.Domain.Exceptions;

namespace SecureSend.Application.Exceptions
{
    internal class FileDoesNotExistException : SecureSendNotFoundException
    {
        public FileDoesNotExistException(string name) : base($"File with name: {name} does not exist.")
        {
        }
    }
}
