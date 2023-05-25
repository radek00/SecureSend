using SecureSend.Application.DTO;
using SecureSend.Application.Exceptions;
using SecureSend.Application.Services;

namespace SecureSend.Application.Queries.Handlers
{
    internal sealed class DownloadFileHandler : IQueryHandler<DownloadFile, FileResultDto>
    {
        private readonly IFileService _fileService;

        public DownloadFileHandler(IFileService fileService)
        {
            _fileService = fileService;
        }

        public Task<FileResultDto> Handle(DownloadFile request, CancellationToken cancellationToken)
        {
            var stream = _fileService.DownloadFile(request.id, request.fileName);
            if (stream == null) throw new NoSavedFileFoundException(request.fileName, request.id);

            return Task.FromResult(
                new FileResultDto
                {
                    FileStream = stream,
                    FileName = request.fileName,
                    ContentType = request.contentType ?? "application/octet-stream"
                }
            );

        }
    }
}
