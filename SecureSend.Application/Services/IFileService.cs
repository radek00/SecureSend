
using Microsoft.AspNetCore.Http;
using SecureSend.Domain.ValueObjects;

namespace SecureSend.Application.Services
{
    public interface IFileService
    {
        FileStream DownloadFile(Guid uploadId, string fileName);
        IEnumerable<string> GetChunksList(Guid uploadId);
        Task MergeFiles(Guid uploadId, IEnumerable<string> chunkFiles);
        Task SaveChunkToDisk(SecureUploadChunk chunk, Guid uploadId);

        void RemovecancelledUpload(Guid uploadId);

    }
}
