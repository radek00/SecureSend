using SecureSend.Domain.Entities;
using SecureSend.Domain.ValueObjects;

namespace SecureSend.Domain.Repositories
{
    public interface ISecureSendUploadRepository
    {
        Task<SecureSendUpload> GetAsync(SecureSendUploadId id, bool track);
        Task AddAsync(SecureSendUpload upload);
        Task DeleteAsync(SecureSendUpload id);
        Task UpdateAsync(SecureSendUpload upload);
        Task SaveChanges();
    }
}
