using SecureSend.Application.DTO;
using SecureSend.Application.Services;

namespace SecureSend.Application.Queries.Handlers;

internal sealed class GetUploadSizeLimitsHandler: IQueryHandler<GetUploadSizeLimits, UploadSizeLimitsResultDto>
{
    private readonly IUploadSizeTrackerService _uploadSizeTrackerService;

    public GetUploadSizeLimitsHandler(IUploadSizeTrackerService uploadSizeTrackerService)
    {
        _uploadSizeTrackerService = uploadSizeTrackerService;
    }


    public Task<UploadSizeLimitsResultDto> Handle(GetUploadSizeLimits request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new UploadSizeLimitsResultDto()
        {
            SingleUploadLimitInGB = _uploadSizeTrackerService.GetUploadLimits().singleUploadLimit / (1024 * 1024 * 1024)
        });
    }
}