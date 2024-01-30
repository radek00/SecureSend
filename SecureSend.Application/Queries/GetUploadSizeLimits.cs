using SecureSend.Application.DTO;

namespace SecureSend.Application.Queries;

public record GetUploadSizeLimits(): IQuery<UploadSizeLimitsResultDto>;