using SecureSend.Application.DTO;
using SecureSend.Application.Exceptions;
using SecureSend.Domain.Repositories;

namespace SecureSend.Application.Commands.Handlers
{
    internal sealed class ViewSecureUploadHandler : ICommandHandler<ViewSecureUpload, SecureUploadDto>
    {
        private readonly ISecureSendUploadRepository _repository;

        public ViewSecureUploadHandler(ISecureSendUploadRepository repository)
        {
            _repository = repository;
        }

        public async Task<SecureUploadDto> Handle(ViewSecureUpload request, CancellationToken cancellationToken)
        {
            var upload = await _repository.GetAsync(request.id, true, cancellationToken);
            if (upload is null) throw new UploadDoesNotExistException(request.id);
            if (upload.ExpiryDate is not null && upload.ExpiryDate < DateTime.UtcNow) throw new UploadExpiredException(upload.ExpiryDate);
            upload.MarkAsViewed();
            await _repository.SaveChanges(cancellationToken);

            var uploadDto = new SecureUploadDto()
            {
                SecureUploadId = upload.Id,
                UploadDate = upload.UploadDate,
                ExpiryDate = upload.ExpiryDate,
                Files = upload.Files.Select(f => new SecureFileDto { ContentType = f.ContentType, FileName = f.FileName })

            };

            return uploadDto;
        }
    }
}
