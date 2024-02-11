using SecureSend.Application.Services;

namespace SecureSend.Infrastructure.Services;

public class DummyUploadTrackerService: IUploadSizeTrackerService
{
    public bool TryUpdateUploadSize(Guid uploadId, double chunkSize)
    {
        return true;
    }

    public void Setup()
    {
    }

    public (double singleUploadLimit, double totalUploadLimit) GetUploadLimits()
    {
        return (0, 0);
    }

    public void Reset()
    {
    }
}