using SecureSend.Application.DTO;
using SecureSend.Application.Exceptions;
using SecureSend.Application.Services;

namespace SecureSend.Application.Queries.Handlers;

public sealed class VerifyUploadHandler: IQueryHandler<VerifyUpload,UploadVerifyResponseDTO>
{
    private readonly ISecureUploadReadService _readService;

    public VerifyUploadHandler(ISecureUploadReadService readService)
    {
        _readService = readService;
    }

    public async Task<UploadVerifyResponseDTO> Handle(VerifyUpload request, CancellationToken cancellationToken)
    {
        var upload = await _readService.GetSecureUpload(request.id, cancellationToken);
        if (upload is null) throw new UploadDoesNotExistException(request.id);
        if (upload.ExpiryDate is not null && upload.ExpiryDate < DateTime.UtcNow) throw new UploadExpiredException(upload.ExpiryDate);

        var uploadVerifyDto = new UploadVerifyResponseDTO()
        {
            SecureUploadId = upload.Id,
            IsProtected = upload.PasswordHash is not null,

        };

        return uploadVerifyDto;
    }
}