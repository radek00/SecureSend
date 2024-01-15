
using Microsoft.AspNetCore.Http;
using SecureSend.Domain.ValueObjects;

namespace SecureSend.Application.Services
{
    public interface IFileService
    {
        FileStream DownloadFile(Guid uploadId, string fileName);
        IEnumerable<string> GetChunksList(Guid uploadId, string chunkDirectory);
        Task MergeFiles(Guid uploadId, IEnumerable<string> chunkFiles, string chunkDirectory, string randomFileName);
        Task SaveChunkToDisk(SecureUploadChunk chunk, Guid uploadId);

        void RemoveUpload(Guid uploadId);
        void RemoveFileFromUpload(Guid uploadId, string fileName);

    }
}
