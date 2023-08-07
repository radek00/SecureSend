using MediatR;
using SecureSend.Application.Exceptions;
using SecureSend.Application.Services;
using SecureSend.Domain.Repositories;

namespace SecureSend.Application.Commands.Handlers
{
    public sealed class DeleteSecureUploadHandler : ICommandHandler<DeleteSecureUpload, Unit>
    {
        private readonly ISecureSendUploadRepository _repository;
        private readonly IFileService _fileService;

        public DeleteSecureUploadHandler(ISecureSendUploadRepository repository, IFileService fileService)
        {
            _repository = repository;
            _fileService = fileService;
        }

        public async Task<Unit> Handle(DeleteSecureUpload request, CancellationToken cancellationToken)
        {
            var persisted = await _repository.GetAsync(request.id, cancellationToken);
            if (persisted is null) throw new UploadDoesNotExistException(request.id);
            _fileService.RemoveUpload(request.id);
            await _repository.DeleteAsync(persisted, cancellationToken);
            return Unit.Value;
        }
    }
}
