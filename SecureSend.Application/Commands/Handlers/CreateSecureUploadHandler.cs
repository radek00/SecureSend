using MediatR;
using SecureSend.Application.Exceptions;
using SecureSend.Application.Services;
using SecureSend.Domain.Factories;
using SecureSend.Domain.Repositories;
using SecureSend.Domain.ValueObjects;

namespace SecureSend.Application.Commands.Handlers
{
    public sealed class CreateSecureUploadHandler: ICommandHandler<CreateSecureUpload, Unit>
    {
        private readonly ISecureSendUploadRepository _secureSendUploadRepository;
        private readonly ISecureSendUploadFactory _secureSendUploadFactory;
        private readonly ISecureUploadReadService _secureUploadReadService;

        public CreateSecureUploadHandler(ISecureSendUploadRepository secureSendUploadRepository,
                                         ISecureSendUploadFactory secureSendUploadFactory,
                                         ISecureUploadReadService secureUploadReadService)
        {
            _secureSendUploadRepository = secureSendUploadRepository;
            _secureSendUploadFactory = secureSendUploadFactory;
            _secureUploadReadService = secureUploadReadService;
        }

        public async Task<Unit> Handle (CreateSecureUpload command, CancellationToken cancellationToken)
        {
            var persisted = await _secureUploadReadService.GetUploadId(command.uploadId, cancellationToken);
            if (persisted != Guid.Empty) throw new UploadAlreadyExistsException(persisted.Value);
            var secureUpload = _secureSendUploadFactory.CreateSecureSendUpload(command.uploadId, new SecureSendUploadDate(), command.expiryDate, false, command.password);
            await _secureSendUploadRepository.AddAsync(secureUpload, cancellationToken);
            return Unit.Value;

        }

    }
}
