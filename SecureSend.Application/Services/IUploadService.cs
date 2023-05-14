
namespace SecureSend.Application.Services
{
    public interface IUploadService
    {
        Task UploadFileChunk(int chunkNumber, int totalChunks, Guid uploadId);
        FileStream DownloadFile(Guid uploadId, string fileName);

    }
}
