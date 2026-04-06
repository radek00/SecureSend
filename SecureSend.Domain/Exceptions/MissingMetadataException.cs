
namespace SecureSend.Domain.Exceptions;

public class MissingMetadataException : SecureSendException
{
    public MissingMetadataException() : base("Metadata must be provided on first chunk")
    {
    }
}
