using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using AutoMapper.QueryableExtensions;
using Lens.Core.Data.EF;
using Lens.Core.Data.EF.Repositories;
using Lens.Core.Lib.Exceptions;
using Lens.Core.Lib.Services;
using Lens.Core.Lib.Models;
using Lens.Services.Masterdata.Models;
using Lens.Services.Masterdata.Repositories;
using System.Linq.Dynamic.Core;

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
        var myQueryModel = querymodel ?? new QueryModel();

        var (query, totalSize) = await this.DbContext.MasterdataTypes
            .GetByQueryModel(myQueryModel)
            .ApplyPaging(myQueryModel);

        var result = await query
            .ProjectTo<MasterdataTypeListModel>(ApplicationService.Mapper.ConfigurationProvider)
            .ToListAsync();

        return new ResultPagedListModel<MasterdataTypeListModel>(result)
        {
            TotalSize = totalSize,
            OriginalQueryModel = myQueryModel
        };
    }

    public async Task<MasterdataTypeModel?> GetMasterdataType(Guid id)
    {
        var result = await this.DbContext.MasterdataTypes
            .ProjectTo<MasterdataTypeModel>(ApplicationService.Mapper.ConfigurationProvider, m => m.Masterdatas).FirstOrDefaultAsync(m => m.Id == id);

        return result;
    }

    public async Task<MasterdataTypeModel?> GetMasterdataType(string code)
    {
        var result = await this.DbContext.MasterdataTypes
            .ProjectTo<MasterdataTypeModel>(ApplicationService.Mapper.ConfigurationProvider, m => m.Masterdatas).FirstOrDefaultAsync(m => m.Code == code);

        return result;
    }

    public async Task<ResultPagedListModel<MasterdataModel>> GetMasterdata(QueryModel? querymodel = null)
    {
        var myQueryModel = querymodel ?? new QueryModel();

        var (query, totalSize) = await this.DbContext.Masterdatas
            .GetByQueryModel(myQueryModel)
            .ApplyPaging(myQueryModel);

        var result = await query
            .ProjectTo<MasterdataModel>(ApplicationService.Mapper.ConfigurationProvider)
            .ToListAsync();

        return new ResultPagedListModel<MasterdataModel>(result)
        {
            TotalSize = totalSize,
            OriginalQueryModel = myQueryModel
        };
    }

    public async Task<ResultPagedListModel<MasterdataModel>> GetMasterdata(string masterdataType, QueryModel? querymodel = null)
    {
        Expression<Func<Entities.Masterdata, bool>> where = null!;
        if (Guid.TryParse(masterdataType, out var masterdataTypeId))
            where = m => m.MasterdataTypeId == masterdataTypeId;
        else
            where = m => m.MasterdataType!.Code == masterdataType;

        var myQueryModel = querymodel ?? new QueryModel();

        var (query, totalSize) = await this.DbContext.Masterdatas
            .Where(where)
            .GetByQueryModel(myQueryModel)
            .ApplyPaging(myQueryModel);

        var result = await query
            .ProjectTo<MasterdataModel>(ApplicationService.Mapper.ConfigurationProvider)
            .ToListAsync();

        return new ResultPagedListModel<MasterdataModel>(result)
        {
            TotalSize = totalSize,
            OriginalQueryModel = myQueryModel
        };
    }

    public async Task<MasterdataModel?> GetMasterdata(string masterdataType, string value)
    {
        Expression<Func<Entities.Masterdata, bool>> whereMasterdataType = null!;
        if (Guid.TryParse(masterdataType, out var masterdataTypeId))
            whereMasterdataType = m => m.MasterdataTypeId == masterdataTypeId;
        else
            whereMasterdataType = m => m.MasterdataType!.Code == masterdataType;

        Expression<Func<Entities.Masterdata, bool>> whereValue = null!;
        if (Guid.TryParse(value, out var valueId))
            whereValue = m => m.Id == valueId;
        else
            whereValue = m => m.Key == value;

        // var result = await base.Get<MasterdataModel>(new QueryModel(), null, Expression.AndAlso(whereMasterdataType, whereValue)); 
        var result = await this.DbContext.Masterdatas
            .Where(whereMasterdataType)
            .Where(whereValue)
            .ProjectTo<MasterdataModel>(ApplicationService.Mapper.ConfigurationProvider).FirstOrDefaultAsync();

        return result;
    }
    #endregion

    #region Add/Post
    public async Task<MasterdataTypeListModel> AddMasterdataType(MasterdataTypeCreateModel model)
    {
        var entry = this.DbContext.MasterdataTypes.Add(ApplicationService.Mapper.Map<Entities.MasterdataType>(model));
        await this.DbContext.SaveChangesAsync();
        return ApplicationService.Mapper.Map<MasterdataTypeListModel>(entry.Entity);
    }

    public async Task<MasterdataModel> AddMasterdata(MasterdataCreateModel model)
    {
        var entry = this.DbContext.Masterdatas.Add(ApplicationService.Mapper.Map<Entities.Masterdata>(model));
        await this.DbContext.SaveChangesAsync();
        return ApplicationService.Mapper.Map<MasterdataModel>(entry.Entity);
    }
    #endregion

    #region Update/Put
    public async Task<MasterdataTypeListModel> UpdateMasterdataType(Guid masterdataTypeId, MasterdataTypeUpdateModel model)
    {
        var dbEntity = await this.DbContext.MasterdataTypes.FindAsync(masterdataTypeId);
        if (dbEntity == default)
        {
            throw new NotFoundException($"MasterdataType with id '{masterdataTypeId}' not found.");
        }

        ApplicationService.Mapper.Map(model, dbEntity);
        await this.DbContext.SaveChangesAsync();
        return ApplicationService.Mapper.Map<MasterdataTypeListModel>(dbEntity);
    }

    public async Task<MasterdataModel> UpdateMasterdata(Guid masterdataId, MasterdataUpdateModel model)
    {
        var dbEntity = await this.DbContext.Masterdatas.FindAsync(masterdataId);
        if (dbEntity == default)
        {
            throw new NotFoundException($"Masterdata with id '{masterdataId}' not found.");
        }

        ApplicationService.Mapper.Map(model, dbEntity);
        await this.DbContext.SaveChangesAsync();
        return ApplicationService.Mapper.Map<MasterdataModel>(dbEntity);
    }
    #endregion

    #region Delete
    public async Task DeleteMasterdataType(Guid id)
    {
        var entity = await this.DbContext.MasterdataTypes.FindAsync(id);
        if (entity == default)
            return;

        this.DbContext.Remove(entity);
        await this.DbContext.SaveChangesAsync();
    }

    public async Task DeleteMasterdata(Guid id)
    {
        //await base.SoftDelete(id); // TODO - not working!

        var entity = await this.DbContext.Masterdatas.FindAsync(id);
        if (entity == default)
            return;

        this.DbContext.Remove(entity);
        await this.DbContext.SaveChangesAsync();
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

        var entry = this.DbContext.MasterdataTypes.Add(ApplicationService.Mapper.Map<Entities.MasterdataType>(newType));

        foreach (var masterdataModel in model.Masterdatas)
        {
            entry.Entity.Masterdatas.Add(ApplicationService.Mapper.Map<Entities.Masterdata>(masterdataModel));
        }

        await this.DbContext.SaveChangesAsync();

        return await GetMasterdataType(entry.Entity.Id);
    }
    #endregion
}
