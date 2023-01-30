using AutoMapper.QueryableExtensions;
using Lens.Core.Data.EF;
using Lens.Core.Data.EF.Repositories;
using Lens.Core.Lib.Exceptions;
using Lens.Core.Lib.Models;
using Lens.Core.Lib.Services;
using Lens.Services.Masterdata.Models;
using Lens.Services.Masterdata.Repositories;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Lens.Services.Masterdata.EF.Repositories;

public class MasterdataRepository : BaseRepository<MasterdataDbContext, Entities.Masterdata>, IMasterdataRepository
{
    public MasterdataRepository(
        MasterdataDbContext dbContext,
        IApplicationService<BaseRepository<MasterdataDbContext>> applicationService
    ) : base(dbContext, applicationService)
    {
    }

    #region Get
    public async Task<ResultPagedListModel<MasterdataTypeListModel>> GetMasterdataTypes(QueryModel? querymodel = null)
    {
        var pagedResult = await DbContext.MasterdataTypes
            .GetByQueryModel(querymodel ?? QueryModel.Default)
            .ApplyPaging(querymodel ?? QueryModel.Default);

        return await pagedResult.ToPagedResultListModel<Entities.MasterdataType, MasterdataTypeListModel>(querymodel ?? QueryModel.Default, ApplicationService.Mapper.ConfigurationProvider);
    }

    public async Task<MasterdataTypeModel?> GetMasterdataType(string masterdataType, string? domain = IMetadataModel.AllDomains)
    {
        var result = await DbContext.MasterdataTypes
            .ProjectTo<MasterdataTypeModel>(ApplicationService.Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(MasterdataTypeModelFilter(masterdataType));

        if (result != null && !string.IsNullOrEmpty(domain))
        {
            result.Domain = domain;
        }

        return result;
    }

    public async Task<ResultPagedListModel<MasterdataModel>> GetMasterdata(string? masterdataType = null, QueryModel? querymodel = null)
    {
        var pagedResult = await DbContext.Masterdatas
            .GetByQueryModel(querymodel ?? QueryModel.Default, MasterdataFilter(masterdataType))
            .ApplyPaging(querymodel ?? QueryModel.Default);

        return await pagedResult.ToPagedResultListModel<Entities.Masterdata, MasterdataModel>(querymodel ?? QueryModel.Default, ApplicationService.Mapper.ConfigurationProvider);
    }

    public async Task<MasterdataModel?> GetMasterdata(string masterdataType, string masterdata)
    {
        // var result = await base.Get<MasterdataModel>(new QueryModel(), null, Expression.AndAlso(whereMasterdataType, whereValue)); 
        var result = await DbContext.Masterdatas
            .Where(MasterdataFilter(masterdataType, masterdata))
            .ProjectTo<MasterdataModel>(ApplicationService.Mapper.ConfigurationProvider).FirstOrDefaultAsync();

        return result;
    }
    #endregion

    #region Add/Post
    public async Task<MasterdataTypeModel> AddMasterdataType(MasterdataTypeCreateModel model)
    {
        var masterdataTypeEntity = await DbContext.MasterdataTypes.FirstOrDefaultAsync(MasterdataTypeFilter(model.Code ?? string.Empty));
        if (masterdataTypeEntity != default)
        {
            throw new BadRequestException($"MasterdataType with code {model.Code} already exists");
        }

        masterdataTypeEntity = ApplicationService.Mapper.Map<Entities.MasterdataType>(model);
        var entry = DbContext.MasterdataTypes.Add(masterdataTypeEntity);
        await DbContext.SaveChangesAsync();
        var result = ApplicationService.Mapper.Map<MasterdataTypeModel>(masterdataTypeEntity);
        if (!string.IsNullOrEmpty(model.Domain))
        {
            result.Domain = model.Domain;
        }
        return result;
    }

    public async Task<MasterdataModel> AddMasterdata(string masterdataType, MasterdataCreateModel model)
    {
        var masterdataTypeEntity = await DbContext.MasterdataTypes.FirstOrDefaultAsync(MasterdataTypeFilter(masterdataType));
        if (masterdataTypeEntity == default)
        {
            throw new NotFoundException($"MasterdataType with id/code {masterdataType} not found.");
        }

        var masterdataEntity = await DbContext.Masterdatas.FirstOrDefaultAsync(MasterdataFilter(masterdataType, model.Key));
        if (masterdataEntity != default)
        {
            throw new NotFoundException($"Masterdata with id/key {model.Key} already exists.");
        }

        masterdataEntity = ApplicationService.Mapper.Map<Entities.Masterdata>(model);
        masterdataTypeEntity.Masterdatas.Add(masterdataEntity);
        await DbContext.SaveChangesAsync();
        return ApplicationService.Mapper.Map<MasterdataModel>(masterdataEntity);
    }
    #endregion

    #region Update/Put
    public async Task<MasterdataTypeModel> UpdateMasterdataType(string masterdataType, MasterdataTypeUpdateModel model)
    {
        var dbEntity = await DbContext.MasterdataTypes.FirstOrDefaultAsync(MasterdataTypeFilter(masterdataType));
        if (dbEntity == default)
        {
            throw new NotFoundException($"MasterdataType with id/code '{masterdataType}' not found.");
        }

        ApplicationService.Mapper.Map(model, dbEntity);
        await DbContext.SaveChangesAsync();
        var result = ApplicationService.Mapper.Map<MasterdataTypeModel>(dbEntity);
        if (!string.IsNullOrEmpty(model.Domain))
        {
            result.Domain = model.Domain;
        }
        return result;
    }

    public async Task<MasterdataModel> UpdateMasterdata(string masterdataType, string masterdata, MasterdataUpdateModel model)
    {
        var entity = await DbContext.Masterdatas.FirstOrDefaultAsync(MasterdataFilter(masterdataType, masterdata));
        if (entity == default)
        {
            throw new NotFoundException($"Masterdata with id/key '{masterdata}' not found.");
        }

        ApplicationService.Mapper.Map(model, entity);
        await DbContext.SaveChangesAsync();
        return ApplicationService.Mapper.Map<MasterdataModel>(entity);
    }
    #endregion

    #region Delete
    public async Task DeleteMasterdataType(string masterdataType)
    {
        var entity = await DbContext.MasterdataTypes.FirstOrDefaultAsync(MasterdataTypeFilter(masterdataType));
        if (entity == default)
        {
            return;
        }

        DbContext.Remove(entity);
        await DbContext.SaveChangesAsync();
    }

    public async Task DeleteMasterdata(string masterdataType, string masterdata)
    {
        //await base.SoftDelete(id); // TODO - not working!

        var entity = await DbContext.Masterdatas.FirstOrDefaultAsync(MasterdataFilter(masterdataType, masterdata));
        if (entity == default)
        {
            return;
        }

        DbContext.Remove(entity);
        await DbContext.SaveChangesAsync();
    }
    #endregion

    #region Others
    public async Task<MasterdataTypeModel?> ImportMasterdata(MasterdataImportModel model)
    {
        var newType = new MasterdataTypeCreateModel
        {
            Name = model.Name,
            Description = model.Description,
            Code = model.Code,
            Metadata = model.Metadata
        };

        var entry = DbContext.MasterdataTypes.Add(ApplicationService.Mapper.Map<Entities.MasterdataType>(newType));

        foreach (var masterdataModel in model.Masterdatas)
        {
            entry.Entity.Masterdatas.Add(ApplicationService.Mapper.Map<Entities.Masterdata>(masterdataModel));
        }

        await DbContext.SaveChangesAsync();

        return await GetMasterdataType(entry.Entity.Id.ToString());
    }
    #endregion

    #region filter helpers

    private static Expression<Func<Entities.Masterdata, bool>> MasterdataFilter(string? masterdataType)
    {
        return string.IsNullOrEmpty(masterdataType)
                    ? m => false
                    : Guid.TryParse(masterdataType, out var masterdataTypeId)
                        ? m => m.MasterdataTypeId == masterdataTypeId
                        : m => m.MasterdataType.Code == masterdataType;
    }

    private static Expression<Func<Entities.Masterdata, bool>> MasterdataFilter(string? masterdataType, string masterdata)
    {
        var filter = MasterdataFilter(masterdataType);

        return filter.And(string.IsNullOrEmpty(masterdata)
            ? m => false
            : Guid.TryParse(masterdata, out var masterdataId)
                        ? m => m.Id == masterdataId
                        : m => m.Key == masterdata);
    }

    private static Expression<Func<MasterdataTypeModel, bool>> MasterdataTypeModelFilter(string masterdataType)
    {
        return string.IsNullOrEmpty(masterdataType)
                    ? m => false
                    : Guid.TryParse(masterdataType, out var id)
                        ? m => m.Id == id
                        : m => m.Code == masterdataType;
    }

    private static Expression<Func<Entities.MasterdataType, bool>> MasterdataTypeFilter(string masterdataType)
    {
        return string.IsNullOrEmpty(masterdataType)
                    ? m => false
                    : Guid.TryParse(masterdataType, out var id)
                        ? m => m.Id == id
                        : m => m.Code == masterdataType;
    }

    #endregion filter helpers
}
