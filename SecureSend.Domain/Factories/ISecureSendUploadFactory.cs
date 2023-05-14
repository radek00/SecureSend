using SecureSend.Domain.Entities;
using SecureSend.Domain.ValueObjects;

namespace SecureSend.Domain.Factories
{
    public interface ISecureSendUploadFactory
    {
        SecureSendUpload CreateSecureSendUpload(SecureSendUploadId id, SecureSendUploadDate uploadDate, SecureSendExpiryDate expiryDate, SecureSendIsViewed isViewedl);
    }
}