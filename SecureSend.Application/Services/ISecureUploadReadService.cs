using SecureSend.Domain.ReadModels;

namespace SecureSend.Application.Services
{
    public interface ISecureUploadReadService
    {
        Task<UploadedFilesReadModel?> GetUploadedFile(string fileName, Guid id, CancellationToken cancellationToken);
        Task<Guid?> GetUploadId(Guid id, CancellationToken cancellationToken);
    }
}
