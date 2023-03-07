using Ganss.Xss;
using Lens.Core.Lib.Models;
using Lens.Core.Lib.Services;
using Lens.Services.Masterdata.Models;
using Lens.Services.Masterdata.Repositories;

namespace Lens.Services.Masterdata.Services;

public class MasterdataService : BaseService<MasterdataService>, IMasterdataService
{
    private readonly HtmlSanitizer htmlSanitizer;
    private readonly IMasterdataRepository _masterdataRepository;

    public MasterdataService(
        IApplicationService<MasterdataService> applicationService,
        IMasterdataRepository masterdataRepository,
        HtmlSanitizer htmlSanitizer
    ) : base(applicationService)
    {
        _masterdataRepository = masterdataRepository;
        this.htmlSanitizer = htmlSanitizer ?? throw new ArgumentNullException(nameof(htmlSanitizer));
    }

    #region Get

    public Task<ResultPagedListModel<MasterdataTypeListModel>> GetMasterdataTypes(QueryModel querymodel)
        => _masterdataRepository.GetMasterdataTypes(querymodel);

    public Task<MasterdataTypeModel?> GetMasterdataType(string masterdataType, string? domain = IMetadataModel.AllDomains)
        => _masterdataRepository.GetMasterdataType(masterdataType, domain);

    public Task<ResultPagedListModel<MasterdataModel>> GetMasterdata(QueryModel querymodel)
        => _masterdataRepository.GetMasterdata(querymodel: querymodel);

    public Task<ResultPagedListModel<MasterdataModel>> GetMasterdata(string masterdataType, QueryModel querymodel)
        => _masterdataRepository.GetMasterdata(masterdataType, querymodel);

    public Task<MasterdataModel?> GetMasterdata(string masterdataType, string value)
        => _masterdataRepository.GetMasterdata(masterdataType, value);

    public Task<ResultPagedListModel<MasterdataKeyModel>> GetMasterdataKeys(string masterdataType, string value, QueryModel querymodel)
        => _masterdataRepository.GetMasterdataKeys(masterdataType, value, querymodel);

    public Task<ResultPagedListModel<string>> GetDomains(string masterdataType, string value, QueryModel querymodel)
        => _masterdataRepository.GetDomains(masterdataType, value, querymodel);

    public Task<ResultPagedListModel<string>> GetTags(string masterdataType, QueryModel querymodel)
        => _masterdataRepository.GetTags(masterdataType, querymodel);
    #endregion Get

    #region Add/Post

    public Task<MasterdataTypeModel> AddMasterdataType(MasterdataTypeCreateModel model)
    {
        model.Sanitize(htmlSanitizer);
        return _masterdataRepository.AddMasterdataType(model);
    }

    public Task<MasterdataModel> AddMasterdata(string masterdataType, MasterdataCreateModel model)
    {
        model.Sanitize(htmlSanitizer);
        return _masterdataRepository.AddMasterdata(masterdataType, model);
    }

    public Task<ICollection<MasterdataKeyModel>> AddMasterdataKeys(string masterdataType, string masterdata, ICollection<MasterdataKeyCreateModel> model)
    {
        model.Sanitize(htmlSanitizer);
        return _masterdataRepository.AddMasterdataKeys(masterdataType, masterdata, model);
    }

    #endregion Add/Post

    #region Update/Put

    public Task<MasterdataTypeModel> UpdateMasterdataType(string masterdataType, MasterdataTypeUpdateModel model)
    {
        model.Sanitize(htmlSanitizer);
        return _masterdataRepository.UpdateMasterdataType(masterdataType, model);
    }

    public Task<MasterdataModel> UpdateMasterdata(string masterdataType, string masterdata, MasterdataUpdateModel model)
    {
        model.Sanitize(htmlSanitizer);
        return _masterdataRepository.UpdateMasterdata(masterdataType, masterdata, model);
    }

    #endregion Update/Put

    #region Delete

    public Task DeleteMasterdataType(string masterdataType)
        => _masterdataRepository.DeleteMasterdataType(masterdataType);

    public Task DeleteMasterdata(string masterdataType, string masterdata)
        => _masterdataRepository.DeleteMasterdata(masterdataType, masterdata);

    public Task DeleteMasterdataKeys(string masterdataType, string masterdata)
        => _masterdataRepository.DeleteMasterdataKeys(masterdataType, masterdata);

    public Task DeleteMasterdataKeys(string masterdataType, string masterdata, Guid alternativeKeyId)
        => _masterdataRepository.DeleteMasterdataKeys(masterdataType, masterdata, alternativeKeyId);
    #endregion Delete

    #region Others

    public Task<MasterdataTypeModel?> ImportMasterdata(MasterdataImportModel model)
        => _masterdataRepository.ImportMasterdata(model);

    #endregion Others
}