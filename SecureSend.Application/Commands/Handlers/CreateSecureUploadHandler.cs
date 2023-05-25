

using MediatR;
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
        private readonly IFileService _fileService;
        private readonly ISecureSendUploadFactory _secureSendUploadFactory;

        public CreateSecureUploadHandler(ISecureSendUploadRepository secureSendUploadRepository,
                                         IFileService fileService,
                                         ISecureSendUploadFactory secureSendUploadFactory)
        {
            _secureSendUploadRepository = secureSendUploadRepository;
            _fileService = fileService;
            _secureSendUploadFactory = secureSendUploadFactory;
        }

        public async Task Handle (CreateSecureUpload command, CancellationToken cancellationToken)
        {
            var secureUpload = _secureSendUploadFactory.CreateSecureSendUpload(command.uploadId, new SecureSendUploadDate(), command.expiryDate, false);
            var chunk = new SecureUploadChunk(command.chunkNumber, command.totalChunks, command.chunk);
            try
            {

                await _fileService.SaveChunkToDisk(chunk, secureUpload.Id);

                if (chunk.IsLast)
                {

                    var savedChunks = _fileService.GetChunksList(secureUpload.Id);
                    if (savedChunks.Count() != chunk.TotalChunks) throw new InvalidChunkCountException(savedChunks.Count(), chunk.TotalChunks);
                    await _fileService.MergeFiles(secureUpload.Id, savedChunks);

                    secureUpload.AddFile(new SecureSendFile(chunk.Chunk.FileName, chunk.ContentType));
                    await _secureSendUploadRepository.AddAsync(secureUpload);


                }
            }
            catch (TaskCanceledException)
            {

                _fileService.RemoveUpload(command.uploadId);
                await _secureSendUploadRepository.DeleteAsync(secureUpload);
            }

        }

    }
}
