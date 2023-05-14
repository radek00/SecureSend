using SecureSend.Domain.Entities;
using SecureSend.Domain.ValueObjects;

namespace SecureSend.Domain.Repositories
{
    public interface ISecureSendUploadRepository
    {
        Task<SecureSendUpload> GetAsync(SecureSendUploadId id);
        Task AddAsync(SecureSendUpload upload);
        Task DeleteAsync(SecureSendUploadId id);
        Task UpdateAsync(SecureSendUpload upload);
    }
}
