using SecureSend.Application.DTO;
using SecureSend.Application.Exceptions;
using SecureSend.Domain.Repositories;

namespace SecureSend.Application.Queries.Handlers
{
    internal sealed class GetSecureUploadHandler: IQueryHandler<GetSecureUpload, SecureUploadDto>
    {
        private readonly ISecureSendUploadRepository _repository;

        public GetSecureUploadHandler(ISecureSendUploadRepository repository)
        {
            _repository = repository;
        }

        public async Task<SecureUploadDto> Handle(GetSecureUpload request, CancellationToken cancellationToken)
        {
            var upload = await _repository.GetAsync(request.id, true);
            if (upload == null) throw new UploadDoesNotExistException(request.id);
            if (upload.ExpiryDate is not null && upload.ExpiryDate < DateTime.UtcNow) throw new UploadExpiredException(upload.ExpiryDate);
            upload.MarkAsViewed();
            await _repository.SaveChanges();

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
