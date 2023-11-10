using MediatR;
using SecureSend.Application.Exceptions;
using SecureSend.Application.Queries.Handlers;
using SecureSend.Application.Services;

internal sealed class CheckIfUploadExistsHandler : IQueryHandler<CheckIfUploadExists, bool>
{
    private readonly ISecureUploadReadService _secureUploadReadService;

    public CheckIfUploadExistsHandler(ISecureUploadReadService secureUploadReadService)
    {
        _secureUploadReadService = secureUploadReadService;
    }

    public async Task<bool> Handle(CheckIfUploadExists request, CancellationToken cancellationToken)
    {
        var uploadId = await _secureUploadReadService.GetUploadId(request.id, cancellationToken);
        return uploadId != Guid.Empty;
    }
}