using SecureSend.Domain.Entities;

namespace SecureSend.Domain.Events
{
    public sealed record SecureUploadViewedEvent(SecureSendUpload upload): IDomainEvent
    {
    }
}
