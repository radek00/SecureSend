

using SecureSend.Application.Services;
using SecureSend.Domain.Factories;
using SecureSend.Domain.Repositories;
using SecureSend.Domain.ValueObjects;

namespace SecureSend.Application.Commands.Handlers
{
    internal sealed class CreateSecureUploadHandler
    {
        private readonly ISecureSendUploadRepository _secureSendUploadRepository;
        private readonly IUploadService _uploadService;
        private readonly ISecureSendUploadFactory _secureSendUploadFactory;

        public CreateSecureUploadHandler(ISecureSendUploadRepository secureSendUploadRepository, IUploadService uploadService)
        {
            _secureSendUploadRepository = secureSendUploadRepository;
            _uploadService = uploadService;
        }

        public async Task HandleAsync (CreateSecureUpload command)
        {
            var secureUpload = _secureSendUploadFactory.CreateSecureSendUpload(command.uploadId, new SecureSendUploadDate(), command.expiryDate, false);

            await _uploadService.UploadFileChunk(command.chunkNumber, command.totalChunks, secureUpload.Id);

            if (command.totalChunks == command.chunkNumber)
            {
                secureUpload.AddFile(new SecureSendFile(command.fileName));
                await _secureSendUploadRepository.AddAsync(secureUpload);
            }
        }
    }
}
