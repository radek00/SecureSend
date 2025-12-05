using MediatR;
using SecureSend.Application.Exceptions;
using SecureSend.Application.Services;
using SecureSend.Domain.Repositories;
using SecureSend.Domain.ValueObjects;

namespace SecureSend.Application.Commands.Handlers
{
    public sealed class UploadChunksHandler : ICommandHandler<UploadChunks, Unit>
    {
        private readonly IFileService _fileService;
        private readonly ISecureSendUploadRepository _repository;
        private readonly IUploadSizeTrackerService _sizeTrackerService;

        public UploadChunksHandler(IFileService fileService, ISecureSendUploadRepository repository, IUploadSizeTrackerService sizeTrackerService)
        {
            _fileService = fileService;
            _repository = repository;
            _sizeTrackerService = sizeTrackerService;
        }

        public async Task<Unit> Handle(UploadChunks command, CancellationToken cancellationToken)
        {
            var chunk = new SecureUploadChunk(command.chunkNumber, command.totalChunks, command.chunk, command.chunkId);
            if (!_sizeTrackerService.TryUpdateUploadSize(command.uploadId, command.chunk.Length))
            {
                
                _fileService.RemoveUpload(command.uploadId);
                _sizeTrackerService.Remove(command.uploadId);
                var persisted = await _repository.GetAsync(command.uploadId, cancellationToken);
                if (persisted is not null) await _repository.DeleteAsync(persisted, cancellationToken);
                throw new SizeLimitExceededException();
            }
            await _fileService.SaveChunkToDisk(chunk, command.uploadId);
            var isComplete = await _fileService.MergeChunks(command.uploadId, chunk.ChunkDirectory, chunk.TotalChunks);

            if (isComplete)
            {
                var persisted = await _repository.GetAsync(command.uploadId, cancellationToken);
                if (persisted is null)
                {
                    _fileService.RemoveUpload(command.uploadId);
                    throw new UploadDoesNotExistException(command.uploadId);
                }
                
                var secureFile = SecureSendFile.Create(chunk.Chunk.FileName, chunk.ContentType, command.totalFileSize);
                await _fileService.FinalizeUpload(persisted.Id, chunk.ChunkDirectory, secureFile.RandomFileName);
                
                
                persisted.AddFile(secureFile);
                await _repository.SaveChanges(cancellationToken);

            }
            return Unit.Value;
        }
    }
}
