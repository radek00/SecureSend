

using SecureSend.Application.Services;
using SecureSend.Domain.Factories;
using SecureSend.Domain.Repositories;
using SecureSend.Domain.ValueObjects;

namespace SecureSend.Application.Commands.Handlers
{
    internal sealed class CreateSecureUploadHandler: ICommandHandler<CreateSecureUpload, object>
    {
        //private readonly ISecureSendUploadRepository _secureSendUploadRepository;
        //private readonly IUploadService _uploadService;
        private readonly ISecureSendUploadFactory _secureSendUploadFactory;

        //public CreateSecureUploadHandler(ISecureSendUploadRepository secureSendUploadRepository, IUploadService uploadService, ISecureSendUploadFactory secureSendUploadFactory)
        //{
        //    //_secureSendUploadRepository = secureSendUploadRepository;
        //    //_uploadService = uploadService;
        //    _secureSendUploadFactory = secureSendUploadFactory;
        //}

        public CreateSecureUploadHandler(ISecureSendUploadFactory secureSendUploadFactory)
        {
            //_secureSendUploadRepository = secureSendUploadRepository;
            //_uploadService = uploadService;
            _secureSendUploadFactory = secureSendUploadFactory;
        }

        public async Task<object> Handle (CreateSecureUpload command, CancellationToken cancellationToken)
        {
            var secureUpload = _secureSendUploadFactory.CreateSecureSendUpload(command.uploadId, new SecureSendUploadDate(), command.expiryDate, false);

            //await _uploadService.UploadFileChunk(command.chunkNumber, command.totalChunks, secureUpload.Id);

            if (command.totalChunks == command.chunkNumber)
            {
                secureUpload.AddFile(new SecureSendFile(command.fileName));
                //await _secureSendUploadRepository.AddAsync(secureUpload);
            }

            return new();
        }

    }
}
