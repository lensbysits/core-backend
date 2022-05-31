using Lens.Services.Masterdata.Models;
using Lens.Core.Lib.Models;

namespace Lens.Services.Masterdata.Services;

public interface IMasterdataService
{
    Task<ResultListModel<MasterdataTypeListModel>> GetMasterdataTypes();
    Task<MasterdataTypeModel?> GetMasterdataType(Guid id);
    Task<MasterdataTypeListModel> AddMasterdataType(MasterdataTypeCreateModel model);
    Task<MasterdataTypeListModel> UpdateMasterdataType(Guid masterdataTypeId, MasterdataTypeUpdateModel model);

    Task<IEnumerable<MasterdataModel>> GetMasterdata(string masterdataType);
    Task<MasterdataModel?> GetMasterdata(string masterdataType, string value);
    Task<MasterdataModel> AddMasterdata(MasterdataCreateModel model);
    Task<MasterdataModel> UpdateMasterdata(Guid masterdataId, MasterdataUpdateModel model);

    Task<MasterdataTypeModel?> ImportMasterdata(MasterdataImportModel model);
    Task DeleteMasterdataType(Guid id);
    Task DeleteMasterdata(Guid id);
    Task<MasterdataTypeModel?> GetMasterdataType(string code);
}