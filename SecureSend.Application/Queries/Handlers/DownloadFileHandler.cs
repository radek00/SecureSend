using SecureSend.Application.DTO;
using SecureSend.Application.Exceptions;
using SecureSend.Application.Services;
using System.Net;

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
            var file = await _secureUploadReadService.GetUploadedFile(WebUtility.HtmlEncode(request.fileName), request.id, cancellationToken);
            if (file == null) throw new FileDoesNotExistException(request.fileName);
            var stream = _fileService.DownloadFile(file.SecureSendUploadId, file.RandomFileName);
            if (stream == null) throw new NoSavedFileFoundException(WebUtility.HtmlEncode(request.fileName), file.SecureSendUploadId);

            return
                new FileResultDto
                {
                    FileStream = stream,
                    FileName = file.DisplayFileName,
                    ContentType = file.ContentType
                };
            

        }
    }
}
