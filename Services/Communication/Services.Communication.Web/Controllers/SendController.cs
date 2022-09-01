using Lens.Services.Communication.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lens.Services.Communication.Web.Controllers;

[Route("[controller]")]
[ApiController]
public class SendController : ControllerBase
{
    private readonly ISenderService _senderService;

    public SendController(ISenderService senderService)
    {
        _senderService = senderService;
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] SendBM sendInfo)
    {
        await _senderService.Send(sendInfo);
        return Ok();
    }
}
