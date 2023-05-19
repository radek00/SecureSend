using SecureSend.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureSend.Application.Exceptions
{
    public class UploadDoesNotExistException : SecureSendException
    {
        public UploadDoesNotExistException(Guid id) : base($"Upload with id: {id} does not exist.")
        {
        }
    }
}
