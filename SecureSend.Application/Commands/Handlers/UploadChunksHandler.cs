using MediatR;
using SecureSend.Application.Exceptions;
using SecureSend.Application.Services;
using SecureSend.Domain.Entities;
using SecureSend.Domain.Repositories;
using SecureSend.Domain.ValueObjects;

namespace SecureSend.Application.Commands.Handlers
{
    internal sealed class UploadChunksHandler : IRequestHandler<UploadChunks>
    {
        private readonly IFileService _fileService;
        private readonly ISecureSendUploadRepository _repository;

        public UploadChunksHandler(IFileService fileService, ISecureSendUploadRepository repository)
        {
            _fileService = fileService;
            _repository = repository;
        }

        public async Task Handle(UploadChunks command, CancellationToken cancellationToken)
        {
            var chunk = new SecureUploadChunk(command.chunkNumber, command.totalChunks, command.chunk);
            SecureSendUpload? persisted = null;

            try
            {
                await _fileService.SaveChunkToDisk(chunk, command.uploadId);

                if (chunk.IsLast)
                {
                    persisted = await _repository.GetAsync(command.uploadId, true);
                    var savedChunks = _fileService.GetChunksList(command.uploadId);
                    if (savedChunks.Count() != chunk.TotalChunks) throw new InvalidChunkCountException(savedChunks.Count(), chunk.TotalChunks);
                    await _fileService.MergeFiles(persisted.Id, savedChunks);

                    
                    persisted.AddFile(new SecureSendFile(chunk.Chunk.FileName, chunk.ContentType));
                    await _repository.SaveChanges();

                }
            }
            catch (TaskCanceledException)
            {

                _fileService.RemoveUpload(command.uploadId);
                if (persisted is not null) await _repository.DeleteAsync(persisted);

            }
        }
    }
}
