using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SecureSend.Application.Services;
using SecureSend.Application.Options;

namespace SecureSend.Infrastructure.Services;

public class UploadSizeTrackerService : IUploadSizeTrackerService
{
    private readonly ConcurrentDictionary<Guid, (double, DateTime)> _uploadSizes = new();
    private readonly Guid _totalSizeId = Guid.NewGuid();
    private readonly double _singleUploadLimit;
    private readonly double _totalUploadLimit;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UploadSizeTrackerService> _logger;

    public UploadSizeTrackerService(IOptions<FileStorageOptions> fileStorageOptions, IServiceProvider serviceProvider, ILogger<UploadSizeTrackerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _totalUploadLimit = fileStorageOptions.Value.TotalUploadLimitInGB * 1024 * 1024 * 1024;
        _singleUploadLimit = fileStorageOptions.Value.SingleUploadLimitInGB * 1024 * 1024 * 1024;
    }

    private double GetInitialTotalSizeValue()
    {
        using var scope = _serviceProvider.CreateScope();
        var fileService = scope.ServiceProvider.GetRequiredService<IFileService>();
        return fileService.GetCurrentUploadDirectorySize();
    }

    public void Setup()
    {
        _uploadSizes.TryAdd(_totalSizeId, (GetInitialTotalSizeValue(), DateTime.UtcNow));
    }

    public void Remove(Guid uploadId)
    {
        var result = _uploadSizes.TryRemove(uploadId, out var removed);
        _uploadSizes.AddOrUpdate(_totalSizeId, (0, DateTime.UtcNow), (key, oldValue) => (oldValue.Item1 - removed.Item1, oldValue.Item2));
        _logger.LogInformation("Removing {upload} from tracker service {status}. Initial creation date: {}, Size: {}", uploadId, result ? "succeeded" :  "failed", removed.Item2, removed.Item1);
    }

    public (double singleUploadLimit, double totalUploadLimit) GetUploadLimits()
    {
        return (_singleUploadLimit, _totalUploadLimit);
    }

    private void AddOrUpdateUploadSize(Guid uploadId, double chunkSize)
    {
        _uploadSizes.AddOrUpdate(uploadId, (chunkSize, DateTime.UtcNow), (key, oldValue) => (oldValue.Item1 + chunkSize, oldValue.Item2));
        _uploadSizes.AddOrUpdate(_totalSizeId, (0, DateTime.UtcNow), (key, oldValue) => (oldValue.Item1 + chunkSize, oldValue.Item2));
    }

    private bool AreSizeLimitsExceeded(Guid uploadId)
    {
        _uploadSizes.TryGetValue(uploadId, out var currentUploadSize);
        _uploadSizes.TryGetValue(_totalSizeId, out var totalUploadSize);

        return !(currentUploadSize.Item1 < _singleUploadLimit) ||
               !(totalUploadSize.Item1 < _totalUploadLimit);
    }

    public bool TryUpdateUploadSize(Guid uploadId, double chunkSize)
    {
        AddOrUpdateUploadSize(uploadId, chunkSize);
        return !AreSizeLimitsExceeded(uploadId);
    }
}