using Lens.Services.Masterdata.Models;
using Lens.Services.Masterdata.Services;
using Lens.Core.Lib.Models;
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
    public async Task<ResultListModel<MasterdataTypeListModel>> Get()
    {
        var result = await _masterdataService.GetMasterdataTypes();
        return result;
    }

    [HttpGet("{masterdataType}")]
    public async Task<IEnumerable<MasterdataModel>> Get(string masterdataType)
    {
        var result = await _masterdataService.GetMasterdata(masterdataType);
        return result;
    }

    [HttpGet("type/{id}")]
    public async Task<MasterdataTypeModel> GetMasterdataType(Guid id)
    {
        var result = await _masterdataService.GetMasterdataType(id);
        return result;
    }

    [HttpPost("type")]
    public async Task<ActionResult<MasterdataTypeModel>> Post(MasterdataTypeCreateModel model)
    {
        var result = await _masterdataService.AddMasterdataType(model);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPost]
    public async Task<ActionResult<MasterdataModel>> Post(MasterdataCreateModel model)
    {
        var result = await _masterdataService.AddMasterdata(model);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("type/{id}")]
    public async Task<MasterdataTypeListModel> Put(Guid id, MasterdataTypeUpdateModel model)
    {
        var result = await _masterdataService.UpdateMasterdataType(id, model);
        return result;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MasterdataModel>> Put(Guid id, MasterdataUpdateModel model)
    {
        var result = await _masterdataService.UpdateMasterdata(id, model);
        return AcceptedAtAction(nameof(Get), new { id = result.Id }, result); 
    }

    [HttpPost("import")]
    public async Task<ActionResult<MasterdataTypeModel>> Import(MasterdataImportModel model)
    {
        var result = await _masterdataService.ImportMasterdata(model);
        return AcceptedAtAction(nameof(Get), new { id = result.Id }, result);
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
