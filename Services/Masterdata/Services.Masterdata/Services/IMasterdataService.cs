using Lens.Services.Masterdata.Models;

namespace Lens.Services.Masterdata.Services;

public interface IMasterdataService
{
    Task<IEnumerable<MasterdataTypeListBM>> GetMasterdataTypes();
    Task<MasterdataTypeBM?> GetMasterdataType(Guid id);
    Task<MasterdataTypeListBM> AddMasterdataType(MasterdataTypeCreateBM model);
    Task<MasterdataTypeListBM> UpdateMasterdataType(Guid masterdataTypeId, MasterdataTypeUpdateBM model);

    Task<IEnumerable<MasterdataBM>> GetMasterdata(string masterdataType);
    Task<MasterdataBM?> GetMasterdata(string masterdataType, string value);
    Task<MasterdataBM> AddMasterdata(MasterdataCreateBM model);
    Task<MasterdataBM> UpdateMasterdata(Guid masterdataId, MasterdataUpdateBM model);

    Task<MasterdataTypeBM?> ImportMasterdata(MasterdataImportBM model);
    Task DeleteMasterdataType(Guid id);
    Task DeleteMasterdata(Guid id);
    Task<MasterdataTypeBM?> GetMasterdataType(string code);
}