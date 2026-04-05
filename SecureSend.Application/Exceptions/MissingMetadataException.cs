using SecureSend.Domain.Exceptions;

namespace SecureSend.Application.Exceptions;

public class MissingMetadataException : SecureSendException
{
    public MissingMetadataException() : base("Metadata must be provided on first chunk")
    {
    }
}
