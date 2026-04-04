using SecureSend.Application.DTO;
using SecureSend.Application.Exceptions;
using SecureSend.Domain.Repositories;

namespace SecureSend.Application.Commands.Handlers
{
    public sealed class ViewSecureUploadHandler : ICommandHandler<ViewSecureUpload, SecureUploadDto>
    {
        private readonly ISecureSendUploadRepository _repository;

        public ViewSecureUploadHandler(ISecureSendUploadRepository repository)
        {
            _repository = repository;
        }

        public async Task<SecureUploadDto> Handle(ViewSecureUpload request, CancellationToken cancellationToken)
        {
            var upload = await _repository.GetAsync(request.id, cancellationToken) ?? throw new UploadDoesNotExistException(request.id);
            if (upload.PasswordHash?.Value != null)
            {
                upload.PasswordHash.VerifyHash(request.password);
            }

            var uploadDto = new SecureUploadDto()
            {
                SecureUploadId = upload.Id,
                UploadDate = upload.UploadDate,
                ExpiryDate = upload.ExpiryDate,
                Files = upload.Files.Select(f => new SecureFileDto 
                { 
                    FileName = f.RandomFileName, 
                    Metadata = f.Metadata
                })

            };

            return uploadDto;
        }
    }
}
