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

        [HttpPut]
        public async Task<ActionResult<SecureUploadDto>> ViewSecureUpload([FromQuery] ViewSecureUpload query, CancellationToken token)
        {
            var result  = await _sender.Send(query, token);
            return OkOrNotFound(result);
        }

        [HttpGet]
        [Route("download")]
        public async Task<FileStreamResult> DownloadFile([FromQuery] DownloadFile file, CancellationToken token)
        {
            var fileStream = await _sender.Send(file, token);
            Response.Headers.Add("Content-Disposition", $"attachment;filename={fileStream.FileName}");
            return new FileStreamResult(fileStream.FileStream, fileStream.ContentType);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromQuery] CreateSecureUpload command, CancellationToken token)
        {
                await _sender.Send(command, token);
                return CreatedAtAction(nameof(Post), new { id = command.uploadId }, null);
            

        }

        [HttpPost]
        [Route("uploadChunks")]
        public async Task<IActionResult> UploadChunks([FromQuery] UploadChunks command, CancellationToken token)
        {
            await _sender.Send(command, token);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] DeleteSecureUpload command, CancellationToken token)
        {
            await _sender.Send(command, token);
            return NoContent();
        }

        [HttpDelete]
        [Route("cancelUpload")]
        public async Task<IActionResult> CancelUpload([FromQuery] CancelUpload command, CancellationToken token)
        {
            await _sender.Send(command, token);
            return NoContent();
        }
    }
}
