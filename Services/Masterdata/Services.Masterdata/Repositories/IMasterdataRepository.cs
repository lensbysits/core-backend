using Lens.Core.Lib.Models;
using Lens.Services.Masterdata.Models;

namespace Lens.Services.Masterdata.Repositories;

public interface IMasterdataRepository
{
    #region Get
    Task<ResultListModel<MasterdataTypeListModel>> GetMasterdataTypes();
    Task<MasterdataTypeModel?> GetMasterdataType(Guid id);
    Task<MasterdataTypeModel?> GetMasterdataType(string code);
    Task<IEnumerable<MasterdataModel>> GetMasterdata();
    Task<IEnumerable<MasterdataModel>> GetMasterdata(string masterdataType);
    Task<MasterdataModel?> GetMasterdata(string masterdataType, string value);
    #endregion

    #region Add/Post
    Task<MasterdataTypeListModel> AddMasterdataType(MasterdataTypeCreateModel model);

    Task<MasterdataModel> AddMasterdata(MasterdataCreateModel model);
    #endregion

    #region Update/Put
    Task<MasterdataTypeListModel> UpdateMasterdataType(Guid masterdataTypeId, MasterdataTypeUpdateModel model);

    Task<MasterdataModel> UpdateMasterdata(Guid masterdataId, MasterdataUpdateModel model);
    #endregion

    #region Delete
    Task DeleteMasterdataType(Guid id);
    
    Task DeleteMasterdata(Guid id);
    #endregion

    #region Others
    Task<MasterdataTypeModel?> ImportMasterdata(MasterdataImportModel model);
    #endregion
}