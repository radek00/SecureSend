using SecureSend.Domain.Entities;
using SecureSend.Domain.ValueObjects;

namespace SecureSend.Domain.Factories
{
    public sealed class SecureSendUploadFactory : ISecureSendUploadFactory
    {
        public SecureSendUploadFactory()
        {

        }

        public SecureSendUpload CreateSecureSendUpload(SecureSendUploadId id, SecureSendUploadDate uploadDate, SecureSendExpiryDate expiryDate, SecureSendIsViewed isViewedl)
        {
            return new SecureSendUpload(id, uploadDate, expiryDate, isViewedl);
        }
    }
}
