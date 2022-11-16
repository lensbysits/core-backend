using Lens.Core.Lib.Models;
using Lens.Core.Lib.Services;
using Lens.Services.Masterdata.Models;
using Lens.Services.Masterdata.Repositories;

namespace Lens.Services.Masterdata.Services;

public class MasterdataService : BaseService<MasterdataService>, IMasterdataService
{
    private readonly IMasterdataRepository _masterdataRepository;

    public MasterdataService(
        IApplicationService<MasterdataService> applicationService,
        IMasterdataRepository masterdataRepository
    ) : base(applicationService)
    {
        _masterdataRepository = masterdataRepository;
    }

    #region Get
    public async Task<ResultListModel<MasterdataTypeListModel>> GetMasterdataTypes()
    {
        var result = await _masterdataRepository.GetMasterdataTypes();
        return result;
    }

    public async Task<MasterdataTypeModel?> GetMasterdataType(Guid id)
    {
        var result = await _masterdataRepository.GetMasterdataType(id);
        return result;
    }

    public async Task<MasterdataTypeModel?> GetMasterdataType(string code)
    {
        var result = await _masterdataRepository.GetMasterdataType(code);
        return result;
    }

    public async Task<IEnumerable<MasterdataModel>> GetMasterdata()
    {
        var result = await _masterdataRepository.GetMasterdata();
        return result;
    }

    public async Task<IEnumerable<MasterdataModel>> GetMasterdata(string masterdataType)
    {
        var result = await _masterdataRepository.GetMasterdata(masterdataType);
        return result;
    }

    public async Task<MasterdataModel?> GetMasterdata(string masterdataType, string value)
    {
        var result = await _masterdataRepository.GetMasterdata(masterdataType, value);
        return result;
    }
    #endregion

    #region Add/Post
    public async Task<MasterdataTypeListModel> AddMasterdataType(MasterdataTypeCreateModel model)
    {
        var result = await _masterdataRepository.AddMasterdataType(model);
        return result;
    }

    public async Task<MasterdataModel> AddMasterdata(MasterdataCreateModel model)
    {
        var result = await _masterdataRepository.AddMasterdata(model);
        return result;
    }
    #endregion

    #region Update/Put
    public async Task<MasterdataTypeListModel> UpdateMasterdataType(Guid masterdataTypeId, MasterdataTypeUpdateModel model)
    {
        var result = await _masterdataRepository.UpdateMasterdataType(masterdataTypeId, model);
        return result;
    }

    public async Task<MasterdataModel> UpdateMasterdata(Guid masterdataId, MasterdataUpdateModel model)
    {
        var result = await _masterdataRepository.UpdateMasterdata(masterdataId, model);
        return result;
    }
    #endregion/Put

    #region Delete
    public async Task DeleteMasterdataType(Guid id)
    {
        await _masterdataRepository.DeleteMasterdataType(id);
    }

    public async Task DeleteMasterdata(Guid id)
    {
        await _masterdataRepository.DeleteMasterdata(id);
    }
    #endregion

    #region Others
    public async Task<MasterdataTypeModel?> ImportMasterdata(MasterdataImportModel model)
    {
        return await _masterdataRepository.ImportMasterdata(model);
    }
    #endregion
}
