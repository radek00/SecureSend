using SecureSend.Application.Exceptions;
using SecureSend.Application.Services;
using SecureSend.Domain.Repositories;

namespace SecureSend.Application.Commands.Handlers
{
    internal sealed class DeleteSecureUploadHandler : ICommandHandler<DeleteSecureUpload>
    {
        private readonly ISecureSendUploadRepository _repository;
        private readonly IFileService _fileService;

        public DeleteSecureUploadHandler(ISecureSendUploadRepository repository, IFileService fileService)
        {
            _repository = repository;
            _fileService = fileService;
        }

        public async Task Handle(DeleteSecureUpload request, CancellationToken cancellationToken)
        {
            var persisted = await _repository.GetAsync(request.id ,true);
            _fileService.RemoveUpload(request.id);
            await _repository.DeleteAsync(persisted);
        }
    }
}
