﻿using Ganss.Xss;
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

    #endregion Delete

    #region Others

    public Task<MasterdataTypeModel?> ImportMasterdata(MasterdataImportModel model)
        => _masterdataRepository.ImportMasterdata(model);

    #endregion Others
}