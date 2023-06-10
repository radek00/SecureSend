using SecureSend.Application.DTO;
using SecureSend.Application.Exceptions;
using SecureSend.Application.Services;

namespace SecureSend.Application.Queries.Handlers
{
    internal sealed class DownloadFileHandler : IQueryHandler<DownloadFile, FileResultDto>
    {
        private readonly IFileService _fileService;
        private readonly ISecureUploadReadService _secureUploadReadService;

        public DownloadFileHandler(IFileService fileService, ISecureUploadReadService secureUploadReadService)
        {
            _fileService = fileService;
            _secureUploadReadService = secureUploadReadService;
        }

        public async Task<FileResultDto> Handle(DownloadFile request, CancellationToken cancellationToken)
        {
            var file = await _secureUploadReadService.GetUploadedFile(request.fileName, request.id, cancellationToken);
            if (file == null) throw new FileDoesNotExistException(request.fileName);
            var stream = _fileService.DownloadFile(file.SecureSendUploadId, file.FileName);
            if (stream == null) throw new NoSavedFileFoundException(file.FileName, file.SecureSendUploadId);

            return
                new FileResultDto
                {
                    FileStream = stream,
                    FileName = file.FileName,
                    ContentType = file.ContentType
                };
            

        }
    }
}
