using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SecureSend.Application.Services;
using SecureSend.Infrastructure.EF.Options;

namespace SecureSend.Infrastructure.Services;

public class UploadSizeTrackerService : IUploadSizeTrackerService
{
    private readonly ConcurrentDictionary<Guid, double> _uploadSizes = new();
    private readonly IOptions<FileStorageOptions> _fileStorageOptions;
    private readonly Guid _totalSizeId = Guid.NewGuid();
    private readonly IServiceProvider _serviceProvider;

    public UploadSizeTrackerService(IOptions<FileStorageOptions> fileStorageOptions, IServiceProvider serviceProvider)
    {
        _fileStorageOptions = fileStorageOptions;
        _serviceProvider = serviceProvider;
    }

    private double GetInitialTotalSizeValue()
    {
        using var scope = _serviceProvider.CreateScope();
        var fileService = scope.ServiceProvider.GetRequiredService<IFileService>();
        return fileService.GetCurrentUploadDirectorySize();
    }

    public void Setup()
    {
        _uploadSizes.TryAdd(_totalSizeId, GetInitialTotalSizeValue());
    }

    private void AddOrUpdateUploadSize(Guid uploadId, double chunkSize)
    {
        _uploadSizes.AddOrUpdate(uploadId, chunkSize, (key, oldValue) => oldValue + chunkSize);
        _uploadSizes.AddOrUpdate(_totalSizeId, 0, (key, oldValue) => oldValue + chunkSize);
    }

    private bool AreSizeLimitsExceeded(Guid uploadId)
    {
        _uploadSizes.TryGetValue(uploadId, out var currentUploadSize);
        _uploadSizes.TryGetValue(_totalSizeId, out var totalUploadSize);

        return !(currentUploadSize < _fileStorageOptions.Value.SingleUploadLimit) ||
               !(totalUploadSize < _fileStorageOptions.Value.TotalUploadLimit);
    }

    public bool TryUpdateUploadSize(Guid uploadId, double chunkSize)
    {
        AddOrUpdateUploadSize(uploadId, chunkSize);
        return !AreSizeLimitsExceeded(uploadId);
    }
}