using MediatR;
using Microsoft.AspNetCore.Mvc;
using SecureSend.Application.Commands;

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

        [HttpPost]
        public async Task<ActionResult<object>> Post([FromQuery] CreateSecureUpload command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        } 
    }
}
