using Lens.Core.Lib.Models;
using Lens.Services.Masterdata.Models;

namespace Lens.Services.Masterdata.Services;

public interface IMasterdataService
{
    #region Get
    Task<ResultPagedListModel<MasterdataTypeListModel>> GetMasterdataTypes(QueryModel querymodel);
    
    Task<MasterdataTypeModel?> GetMasterdataType(string masterdataType, string? domain = IMetadataModel.AllDomains);
    
    Task<ResultPagedListModel<MasterdataModel>> GetMasterdata(MasterdataQueryModel querymodel);
    
    Task<ResultPagedListModel<MasterdataModel>> GetMasterdata(string masterdataType, MasterdataQueryModel querymodel);
    
    Task<MasterdataModel?> GetMasterdata(string masterdataType, string masterdata);
    
    Task<ResultPagedListModel<MasterdataKeyModel>> GetMasterdataKeys(string masterdataType, string masterdata, QueryModel querymodel);

    Task<ResultPagedListModel<string>> GetDomains(string masterdataType, string masterdata, QueryModel querymodel);

    Task<ResultPagedListModel<string>> GetTags(string masterdataType, QueryModel querymodel);

    Task<ResultListModel<MasterdataModel>> GetMasterdataRelated(string masterdataType, string masterdata, string? relatedMasterdataType = null, bool includeDescendants = false);

    #endregion

    #region Add/Post
    Task<MasterdataTypeModel> AddMasterdataType(MasterdataTypeCreateModel model);

    Task<MasterdataModel> AddMasterdata(string masterdataType, MasterdataCreateModel model);

    Task<ICollection<MasterdataKeyModel>> AddMasterdataKeys(string masterdataType, string masterdata, ICollection<MasterdataKeyCreateModel> model);
    Task<ICollection<MasterdataRelatedModel>> AddMasterdataRelated(string masterdataType, string masterdata, ICollection<MasterdataRelatedCreateModel> model);
    #endregion

    #region Update/Put
    Task<MasterdataTypeModel> UpdateMasterdataType(string masterdataType, MasterdataTypeUpdateModel model);

    Task<MasterdataModel> UpdateMasterdata(string masterdataType, string masterdata, MasterdataUpdateModel model);
    #endregion

    #region Delete
    Task DeleteMasterdataType(string masterdataType);

    Task DeleteMasterdata(string masterdataType, string masterdata);

    Task DeleteMasterdataKeys(string masterdataType, string masterdata);

    Task DeleteMasterdataKeys(string masterdataType, string masterdata, Guid alternativeKeyId);

    Task DeleteMasterdataRelated(string masterdataType, string masterdata, List<Guid> relatrelatedMasterdataIdsedIds);

    #endregion

    #region Others
    Task<MasterdataTypeModel?> ImportMasterdata(MasterdataImportModel model);
    #endregion
}