using SecureSend.Application.Exceptions;
using SecureSend.Application.Services;
using SecureSend.Domain.Factories;
using SecureSend.Domain.Repositories;
using SecureSend.Domain.ValueObjects;

namespace SecureSend.Application.Commands.Handlers
{
    internal sealed class CreateSecureUploadHandler: ICommandHandler<CreateSecureUpload>
    {
        private readonly ISecureSendUploadRepository _secureSendUploadRepository;
        private readonly ISecureSendUploadFactory _secureSendUploadFactory;

        public CreateSecureUploadHandler(ISecureSendUploadRepository secureSendUploadRepository,
                                         IFileService fileService,
                                         ISecureSendUploadFactory secureSendUploadFactory)
        {
            _secureSendUploadRepository = secureSendUploadRepository;
            _secureSendUploadFactory = secureSendUploadFactory;
        }

        public async Task Handle (CreateSecureUpload command, CancellationToken cancellationToken)
        {
            var persisted = await _secureSendUploadRepository.GetAsync(command.uploadId, false, cancellationToken);
            if (persisted is not null) throw new UploadAlreadyExistsException(persisted.Id);
            var secureUpload = _secureSendUploadFactory.CreateSecureSendUpload(command.uploadId, new SecureSendUploadDate(), command.expiryDate, false);
            await _secureSendUploadRepository.AddAsync(secureUpload, cancellationToken);

        }

    }
}
