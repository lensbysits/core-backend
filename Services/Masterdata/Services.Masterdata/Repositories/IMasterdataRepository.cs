using Lens.Core.Lib.Models;
using Lens.Services.Masterdata.Models;

namespace Lens.Services.Masterdata.Repositories;

public interface IMasterdataRepository
{
    #region Get
    Task<ResultPagedListModel<MasterdataTypeListModel>> GetMasterdataTypes(QueryModel? querymodel = null);
    Task<MasterdataTypeModel?> GetMasterdataType(string masterdataType);
    Task<ResultPagedListModel<MasterdataModel>> GetMasterdata(string? masterdataType = null, QueryModel? querymodel = null);
    Task<MasterdataModel?> GetMasterdata(string masterdataType, string value);
    #endregion

    #region Add/Post
    Task<MasterdataTypeListModel> AddMasterdataType(MasterdataTypeCreateModel model);

    Task<MasterdataModel> AddMasterdata(string masterdataType, MasterdataCreateModel model);
    #endregion

    #region Update/Put
    Task<MasterdataTypeListModel> UpdateMasterdataType(string masterdataType, MasterdataTypeUpdateModel model);

    Task<MasterdataModel> UpdateMasterdata(string masterdataType, string masterdata, MasterdataUpdateModel model);
    #endregion

    #region Delete
    Task DeleteMasterdataType(string masterdataType);
    
    Task DeleteMasterdata(string masterdataType, string masterdata);
    #endregion

    #region Others
    Task<MasterdataTypeModel?> ImportMasterdata(MasterdataImportModel model);
    #endregion
}