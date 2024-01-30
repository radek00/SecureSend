using MediatR;
using Microsoft.AspNetCore.Mvc;
using SecureSend.Application.DTO;
using SecureSend.Application.Queries;

namespace SecureSend.Controllers;

[ApiController]
[Route("api/uploadLimits")]
public class LimitsController: BaseController
{
    private readonly ISender _sender;

    public LimitsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<UploadSizeLimitsResultDto> Get(CancellationToken token)
    {
        var limits = await _sender.Send(new GetUploadSizeLimits(), token);
        return limits;
    }
}