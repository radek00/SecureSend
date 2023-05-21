using SecureSend.Application.DTO;
using SecureSend.Application.Exceptions;
using SecureSend.Application.Services;

namespace SecureSend.Application.Queries.Handlers
{
    internal sealed class DownloadFileHandler : IQueryHandler<DownloadFile, FileResultDto>
    {
        private readonly IFileService _fileService;

        public Task<FileResultDto> Handle(DownloadFile request, CancellationToken cancellationToken)
        {
            var stream = _fileService.DownloadFile(request.id, request.fileName);

            if (stream == null) throw new NoSavedFileFoundException(request.fileName, request.id);
        }
    }
}
