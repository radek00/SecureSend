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
        public async Task<ActionResult<SecureUploadDto>> Get([FromQuery] GetSecureUpload query)
        {
            var result  = await _sender.Send(query);
            return OkOrNotFound(result);
        }

        [HttpGet]
        [Route("[controller]/download")]
        public async Task<FileStreamResult> DownloadFile([FromQuery] DownloadFile file)
        {
            var fileStream = await _sender.Send(file);
            return new FileStreamResult(fileStream.FileStream, fileStream.ContentType);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromQuery] CreateSecureUpload command)
        {
            await _sender.Send(command);
            return CreatedAtAction(nameof(Post), new { id = command.uploadId }, null);
        } 
    }
}
