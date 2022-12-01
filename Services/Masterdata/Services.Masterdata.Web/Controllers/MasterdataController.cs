using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Lens.Core.Lib.Models;
using Lens.Services.Masterdata.Models;
using Lens.Services.Masterdata.Services;

namespace Services.Masterdata.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class MasterdataController : ControllerBase
{

    private readonly IMasterdataService _masterdataService;

    public MasterdataController(IMasterdataService masterdataService)
    {
        _masterdataService = masterdataService;
    }

    #region HttpGet
    /// <summary>
    /// List all masterdata types.
    /// </summary>
    /// <param name="queryModel">The settings for paging, sorting and filtering.</param>
    /// <returns>A list of masterdata types.</returns>
    [HttpGet("type/")]
    public async Task<ResultPagedListModel<MasterdataTypeListModel>> Get([FromQuery] QueryModel queryModel)
    {
        var result = await _masterdataService.GetMasterdataTypes(queryModel);
        return result;
    }

    [HttpGet("type/{id}")]
    public async Task<MasterdataTypeModel?> GetMasterdataType(Guid id)
    {
        var result = await _masterdataService.GetMasterdataType(id);
        return result;
    }

    [HttpGet("type/code={code}")]
    public async Task<MasterdataTypeModel?> GetMasterdataType(string code)
    {
        var result = await _masterdataService.GetMasterdataType(code);
        return result;
    }

    /// <summary>
    /// List all masterdatas.
    /// </summary>
    /// <param name="queryModel">The settings for paging, sorting and filtering.</param>
    /// <returns>A list of masterdatas.</returns>
    [HttpGet()]
    public async Task<ResultPagedListModel<MasterdataModel>> GetMasterdata([FromQuery] QueryModel queryModel)
    {
        var result = await _masterdataService.GetMasterdata(queryModel);
        return result;
    }

    /// <summary>
    /// List all masterdatas belonging to a specific masterdata type.
    /// </summary>
    /// <param name="queryModel">The settings for paging, sorting and filtering.</param>
    /// <returns>A list of masterdatas belonging to a specific masterdata type.</returns>
    [HttpGet("{masterdataType}")]
    public async Task<ResultPagedListModel<MasterdataModel>> GetMasterdata(string masterdataType, [FromQuery] QueryModel queryModel)
    {
        var result = await _masterdataService.GetMasterdata(masterdataType, queryModel);
        return result;
    }

    [HttpGet("{masterdataType}/{value}")]
    public async Task<MasterdataModel?> GetMasterdata(string masterdataType, string value)
    {
        var result = await _masterdataService.GetMasterdata(masterdataType, value);
        return result;
    }
    #endregion

    #region HttpPost
    [HttpPost("type")]
    public async Task<ActionResult<MasterdataTypeListModel>> Post(MasterdataTypeCreateModel model)
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
    #endregion

    #region HttpPut
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
    #endregion

    #region HttpDelete
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
    #endregion

    #region Others
    [HttpPost("import")]
    public async Task<ActionResult<MasterdataTypeModel>> Import(MasterdataImportModel model)
    {
        var result = await _masterdataService.ImportMasterdata(model);
        return AcceptedAtAction(nameof(Get), new { id = result?.Id }, result);
    }
    #endregion
}
