using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Lens.Core.Lib.Models;
using Lens.Services.Masterdata.Models;
using Lens.Services.Masterdata.Services;

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

    #region HttpGet
    /// <summary>
    /// List all masterdata types.
    /// </summary>
    /// <param name="queryModel">The settings for paging, sorting and filtering.</param>
    /// <returns>A list of masterdata types.</returns>
    [HttpGet]
    public async Task<ResultPagedListModel<MasterdataTypeListModel>> Get([FromQuery] QueryModel queryModel)
    {
        var result = await _masterdataService.GetMasterdataTypes(queryModel);
        return result;
    }

    /// <summary>
    /// Show the details of a masterdata type.
    /// </summary>
    /// <param name="masterdataType">The masterdata type (Id or Code).</param>
    /// <param name="domain">Application domain.</param>
    /// <returns>The details for a masterdata type.</returns>
    [HttpGet("{masterdataType}/details")]
    public async Task<MasterdataTypeModel?> GetMasterdataType(string masterdataType, [FromHeader(Name = "masterdata-domain")]string? domain)
    {
        var result = await _masterdataService.GetMasterdataType(masterdataType, domain);
        return result;
    }

    /// <summary>
    /// List all masterdatas belonging to a specific masterdata type.
    /// </summary>
    /// <param name="masterdataType">The masterdata type (Id or Code).</param>
    /// <param name="queryModel">The settings for paging, sorting and filtering.</param>
    /// <returns>A list of masterdatas belonging to a specific masterdata type.</returns>
    [HttpGet("{masterdataType}")]
    public async Task<ResultPagedListModel<MasterdataModel>> GetMasterdata(string masterdataType, [FromQuery] QueryModel queryModel)
    {
        var result = await _masterdataService.GetMasterdata(masterdataType, queryModel);
        return result;
    }

    /// <summary>
    /// Show the details of a masterdata item.
    /// </summary>
    /// <param name="masterdataType">The masterdata type.</param>
    /// <param name="value">The masterdata item identifier (Id or Key).</param>
    /// <returns>The details for a masterdata item belonging to a specific masterdata type.</returns>
    [HttpGet("{masterdataType}/{value}")]
    public async Task<MasterdataModel?> GetMasterdata(string masterdataType, string value)
    {
        var result = await _masterdataService.GetMasterdata(masterdataType, value);
        return result;
    }

    /// <summary>
    /// List all tags associated with a specific masterdata type.
    /// </summary>
    /// <param name="masterdataType">The masterdata type (Id or Code).</param>
    /// <param name="queryModel">The settings for paging, sorting and filtering.</param>
    /// <returns>A list of tags associated with a specific masterdata type.</returns>
    [HttpGet("{masterdataType}/tags")]
    public async Task<ResultPagedListModel<string>> GetTags(string masterdataType, [FromQuery] QueryModel queryModel)
    {
        var result = await _masterdataService.GetTags(masterdataType, queryModel);
        return result;
    }
    #endregion

    #region HttpPost
    [HttpPost]
    public async Task<ActionResult<MasterdataTypeModel>> Post(MasterdataTypeCreateModel model)
    {
        var result = await _masterdataService.AddMasterdataType(model);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPost("{masterdataType}")]
    public async Task<ActionResult<MasterdataModel>> Post(string masterdataType, MasterdataCreateModel model)
    {
        var result = await _masterdataService.AddMasterdata(masterdataType, model);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPost("{masterdataType}/{masterdata}/keys")]
    public async Task<ActionResult<MasterdataModel>> Post(string masterdataType, string masterdata, MasterdataKeyCreateModel model)
    {
        var result = await _masterdataService.AddMasterdataKeys(masterdataType, masterdata, model);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }
    #endregion

    #region HttpPut
    [HttpPut("{masterdataType}/details")]
    public async Task<MasterdataTypeModel> Put(string masterdataType, MasterdataTypeUpdateModel model)
    {
        var result = await _masterdataService.UpdateMasterdataType(masterdataType, model);
        return result;
    }

    [HttpPut("{masterdataType}/{masterdata}")]
    public async Task<ActionResult<MasterdataModel>> Put(string masterdataType, string masterdata, MasterdataUpdateModel model)
    {
        var result = await _masterdataService.UpdateMasterdata(masterdataType, masterdata, model);
        return AcceptedAtAction(nameof(Get), new { id = result.Id }, result);
    }
    #endregion

    #region HttpDelete
    [HttpDelete("{masterdataType}/details")]
    public async Task<ActionResult> DeleteType(string masterdataType)
    {
        await _masterdataService.DeleteMasterdataType(masterdataType);
        return Ok();
    }

    [HttpDelete("{masterdataType}/{masterdata}/keys")]
    public async Task<ActionResult> DeleteMasterdataKeys(string masterdataType, string masterdata)
    {
        await _masterdataService.DeleteMasterdataKeys(masterdataType, masterdata);
        return Ok();
    }

    [HttpDelete("{masterdataType}/{masterdata}/keys/{alternativeKeyId}")]
    public async Task<ActionResult> DeleteMasterdataKeys(string masterdataType, string masterdata, string alternativeKeyId)
    {
        await _masterdataService.DeleteMasterdataKeys(masterdataType, masterdata, alternativeKeyId);
        return Ok();
    }

    [HttpDelete("{masterdataType}/{masterdata}")]
    public async Task<ActionResult> Delete(string masterdataType, string masterdata)
    {
        await _masterdataService.DeleteMasterdata(masterdataType, masterdata);
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
