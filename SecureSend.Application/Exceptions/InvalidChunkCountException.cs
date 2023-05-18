using SecureSend.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureSend.Application.Exceptions
{
    public class InvalidChunkCountException : SecureSendException
    {
        public InvalidChunkCountException(int countedChunks, int expectedChunks) : base($"Counted chunks: {countedChunks} are not equal to expected chunks: {expectedChunks}")
        {
        }
    }
}
