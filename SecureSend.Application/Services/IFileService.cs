using Microsoft.AspNetCore.Http;
using SecureSend.Domain.ValueObjects;

namespace SecureSend.Application.Services
{
    public interface IFileService
    {
        FileStream? DownloadFile(Guid uploadId, string fileName);
        Task<SecureSendFile?> HandleChunk(SecureUploadChunk chunk, Guid uploadId, long totalFileSize);

        void RemoveUpload(Guid uploadId);
        void RemoveFileFromUpload(Guid uploadId, string fileName);
        double GetCurrentUploadDirectorySize();

        void SetupUploadDirectory(Guid uploadId);
    }
}
