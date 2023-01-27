using Lens.Core.Lib.Models;
using Lens.Services.Masterdata.Models;

namespace Lens.Services.Masterdata.Services;

public interface IMasterdataService
{
    #region Get
    Task<ResultPagedListModel<MasterdataTypeListModel>> GetMasterdataTypes(QueryModel querymodel);
    Task<MasterdataTypeModel?> GetMasterdataType(string masterdataType, string? domain = IMetadataModel.AllDomains);
    Task<ResultPagedListModel<MasterdataModel>> GetMasterdata(QueryModel querymodel);
    Task<ResultPagedListModel<MasterdataModel>> GetMasterdata(string masterdataType, QueryModel querymodel);
    Task<MasterdataModel?> GetMasterdata(string masterdataType, string masterdata);
    Task<ResultPagedListModel<string>> GetTags(string masterdataType, QueryModel querymodel);
    #endregion

    #region Add/Post
    Task<MasterdataTypeModel> AddMasterdataType(MasterdataTypeCreateModel model);

    Task<MasterdataModel> AddMasterdata(string masterdataType, MasterdataCreateModel model);
    #endregion

    #region Update/Put
    Task<MasterdataTypeModel> UpdateMasterdataType(string masterdataType, MasterdataTypeUpdateModel model);

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