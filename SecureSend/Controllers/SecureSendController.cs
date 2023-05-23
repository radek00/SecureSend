using MediatR;
using Microsoft.AspNetCore.Mvc;
using SecureSend.Application.Commands;
using SecureSend.Application.DTO;
using SecureSend.Application.Queries;

namespace SecureSend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecureSendController: BaseController
    {
        protected readonly ISender _sender;

        public SecureSendController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<ActionResult<SecureUploadDto>> Get([FromQuery] GetSecureUpload query, CancellationToken token)
        {
            var result  = await _sender.Send(query, token);
            return OkOrNotFound(result);
        }

        [HttpGet]
        [Route("[controller]/download")]
        public async Task<FileStreamResult> DownloadFile([FromQuery] DownloadFile file, CancellationToken token)
        {
            var fileStream = await _sender.Send(file, token);
            return new FileStreamResult(fileStream.FileStream, fileStream.ContentType);
        }

        [HttpPost]
        [RequestSizeLimit(10L * 1024L * 1024L * 1024L)]
        [RequestFormLimits(MultipartBodyLengthLimit = 10L * 1024L * 1024L * 1024L)]
        public async Task<IActionResult> Post([FromQuery] CreateSecureUpload command, CancellationToken token)
        {
                await _sender.Send(command, token);
                return CreatedAtAction(nameof(Post), new { id = command.uploadId }, null);
            

        }


    }
}
