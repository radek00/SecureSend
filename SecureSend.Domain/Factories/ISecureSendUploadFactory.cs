using SecureSend.Domain.Entities;
using SecureSend.Domain.ValueObjects;

namespace SecureSend.Domain.Factories
{
    public interface ISecureSendUploadFactory
    {
        SecureSendUpload CreateSecureSendUpload(Guid id, DateTime? expiryDate, bool isViewed, string? password);
    }
}