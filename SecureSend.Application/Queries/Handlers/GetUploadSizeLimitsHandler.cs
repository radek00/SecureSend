using Microsoft.Extensions.Options;
using SecureSend.Application.DTO;
using SecureSend.Application.Options;
using SecureSend.Application.Services;

namespace SecureSend.Application.Queries.Handlers;

internal sealed class GetUploadSizeLimitsHandler: IQueryHandler<GetUploadSizeLimits, UploadSizeLimitsResultDto>
{
    private readonly IUploadSizeTrackerService _uploadSizeTrackerService;
    private readonly IOptions<FileStorageOptions> _fileStorageOptions;

    public GetUploadSizeLimitsHandler(IUploadSizeTrackerService uploadSizeTrackerService, IOptions<FileStorageOptions> fileStorageOptions)
    {
        _uploadSizeTrackerService = uploadSizeTrackerService;
        _fileStorageOptions = fileStorageOptions;
    }


    public Task<UploadSizeLimitsResultDto> Handle(GetUploadSizeLimits request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new UploadSizeLimitsResultDto()
        {
            SingleUploadLimitInGB = _uploadSizeTrackerService.GetUploadLimits().singleUploadLimit / (1024 * 1024 * 1024),
            MaxExpirationInDays = _fileStorageOptions.Value.MaxExpirationInDays
        });
    }
}