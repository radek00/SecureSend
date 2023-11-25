using SecureSend.Domain.Entities;
using SecureSend.Domain.ValueObjects;

namespace SecureSend.Domain.Repositories
{
    public interface ISecureSendUploadRepository
    {
        Task<SecureSendUpload?> GetAsync(SecureSendUploadId id, CancellationToken cancellationToken);
        Task AddAsync(SecureSendUpload upload, CancellationToken cancellationToken);
        Task DeleteAsync(SecureSendUpload id, CancellationToken cancellationToken);
        Task UpdateAsync(SecureSendUpload upload, CancellationToken cancellationToken);
        Task SaveChanges(CancellationToken cancellationToken);
    }
}
