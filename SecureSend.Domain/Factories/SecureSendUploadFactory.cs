using SecureSend.Domain.Entities;
using SecureSend.Domain.ValueObjects;

namespace SecureSend.Domain.Factories
{
    public sealed class SecureSendUploadFactory : ISecureSendUploadFactory
    {
        public SecureSendUploadFactory()
        {

        }

        public SecureSendUpload CreateSecureSendUpload(Guid id, DateTime? expiryDate, bool isViewed, string? password)
        {
            return new SecureSendUpload(SecureSendUploadId.Create(id), SecureSendUploadDate.Create(),
                SecureSendExpiryDate.Create(expiryDate), new SecureSendIsViewed(isViewed),
                SecureSendPasswordHash.Create(password));
        }
    }
}
