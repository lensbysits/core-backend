using Lens.Core.Lib.Models;
using Lens.Services.Masterdata.Models;

namespace Lens.Services.Masterdata.Repositories;

public interface IMasterdataRepository
{
    #region Get
    Task<ResultPagedListModel<MasterdataTypeListModel>> GetMasterdataTypes(QueryModel? querymodel = null);
    Task<MasterdataTypeModel?> GetMasterdataType(Guid id);
    Task<MasterdataTypeModel?> GetMasterdataType(string code);
    Task<ResultPagedListModel<MasterdataModel>> GetMasterdata(QueryModel? querymodel = null);
    Task<ResultPagedListModel<MasterdataModel>> GetMasterdata(string masterdataType, QueryModel? querymodel = null);
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