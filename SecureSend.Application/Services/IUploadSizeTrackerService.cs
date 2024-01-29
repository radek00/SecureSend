namespace SecureSend.Application.Services;

public interface IUploadSizeTrackerService
{
    bool TryUpdateUploadSize(Guid uploadId, double chunkSize);
    void Setup();
}