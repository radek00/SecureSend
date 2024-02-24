using MediatR;
using Microsoft.Extensions.Options;
using SecureSend.Application.Exceptions;
using SecureSend.Application.Options;
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
        private readonly IOptions<FileStorageOptions> _fileStorageOptions;
        private readonly IUploadSizeTrackerService _sizeTrackerService;

        public CreateSecureUploadHandler(ISecureSendUploadRepository secureSendUploadRepository,
                                         ISecureSendUploadFactory secureSendUploadFactory,
                                         ISecureUploadReadService secureUploadReadService, IOptions<FileStorageOptions> fileStorageOptions, IUploadSizeTrackerService sizeTrackerService)
        {
            _secureSendUploadRepository = secureSendUploadRepository;
            _secureSendUploadFactory = secureSendUploadFactory;
            _secureUploadReadService = secureUploadReadService;
            _fileStorageOptions = fileStorageOptions;
            _sizeTrackerService = sizeTrackerService;
        }

        public async Task<Unit> Handle (CreateSecureUpload command, CancellationToken cancellationToken)
        {
            var expiryDate = command.expiryDate;
            if (_fileStorageOptions.Value.MaxExpirationInDays != 0 &&
                expiryDate > DateTime.UtcNow.Date.AddDays(_fileStorageOptions.Value.MaxExpirationInDays).Date)
                throw new MaxExpirationExceededException(_fileStorageOptions.Value.MaxExpirationInDays);
            if (_fileStorageOptions.Value.MaxExpirationInDays != 0 && expiryDate == null)
            {
                expiryDate = DateTime.UtcNow.Date.AddDays(_fileStorageOptions.Value.MaxExpirationInDays);
            }
            var persisted = await _secureUploadReadService.GetUploadId(command.uploadId, cancellationToken);
            if (persisted is not null && persisted != Guid.Empty) throw new UploadAlreadyExistsException(persisted.Value);
            _sizeTrackerService.TryUpdateUploadSize(command.uploadId, 0);
            var secureUpload = _secureSendUploadFactory.CreateSecureSendUpload(command.uploadId, expiryDate, false, command.password);
            await _secureSendUploadRepository.AddAsync(secureUpload, cancellationToken);
            return Unit.Value;

        }

    }
}
