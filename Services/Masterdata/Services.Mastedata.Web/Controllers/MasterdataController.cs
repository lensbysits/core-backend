using Lens.Services.Masterdata.Models;
using Lens.Services.Masterdata.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Services.Masterdata.Web.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class MasterdataController : ControllerBase
{

    private readonly IMasterdataService _masterdataService;

    public MasterdataController(IMasterdataService masterdataService)
    {
        _masterdataService = masterdataService;
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        var result = await _masterdataService.GetMasterdataTypes();
        return Ok(result);
    }

    [HttpGet("{masterdataType}")]
    public async Task<ActionResult> Get(string masterdataType)
    {
        var result = await _masterdataService.GetMasterdata(masterdataType);
        return Ok(result);
    }

    [HttpGet("type/{id}")]
    public async Task<ActionResult> GetMasterdataType(Guid id)
    {
        var result = await _masterdataService.GetMasterdataType(id);
        return Ok(result);
    }

    [HttpPost("type")]
    public async Task<ActionResult> Post(MasterdataTypeCreateBM model)
    {
        var result = await _masterdataService.AddMasterdataType(model);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> Post(MasterdataCreateBM model)
    {
        var result = await _masterdataService.AddMasterdata(model);
        return Ok(result);
    }

    [HttpPut("type/{id}")]
    public async Task<ActionResult> Put(Guid id, MasterdataTypeUpdateBM model)
    {
        var result = await _masterdataService.UpdateMasterdataType(id, model);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(Guid id, MasterdataUpdateBM model)
    {
        var result = await _masterdataService.UpdateMasterdata(id, model);
        return Ok(result);
    }

    [HttpPost("import")]
    public async Task<ActionResult> Import(MasterdataImportBM model)
    {
        var result = await _masterdataService.ImportMasterdata(model);
        return Ok(result);
    }

    [HttpDelete("type/{id}")]
    public async Task<ActionResult> DeleteType(Guid id)
    {
        await _masterdataService.DeleteMasterdataType(id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _masterdataService.DeleteMasterdata(id);
        return Ok();
    }
}
