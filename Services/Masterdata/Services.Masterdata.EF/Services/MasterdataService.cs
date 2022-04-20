using AutoMapper.QueryableExtensions;
using Lens.Core.Lib.Exceptions;
using Lens.Core.Lib.Services;
using Lens.Core.Lib.Models;
using Lens.Services.Masterdata.EF.Entities;
using Lens.Services.Masterdata.Models;
using Lens.Services.Masterdata.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Lens.Core.Data.EF.Services;
namespace Lens.Services.Masterdata.EF.Services;

public class MasterdataService : DataService<MasterdataService, Entities.Masterdata, MasterdataDbContext>, IMasterdataService
{
    public MasterdataService(MasterdataDbContext masterdataDbContext, IApplicationService<MasterdataService> applicationService) : base(applicationService, masterdataDbContext)
    {
    }

    public async Task<ResultListModel<MasterdataTypeListModel>> GetMasterdataTypes()
    {
        var result = await ApplicationDbContext.MasterdataTypes
            .ProjectTo<MasterdataTypeListModel>(ApplicationService.Mapper.ConfigurationProvider).ToListAsync();

        return new ResultListModel<MasterdataTypeListModel>
        {
            Value = result,
            Size = result.Count
        };
    }

    public async Task<MasterdataTypeModel?> GetMasterdataType(Guid id)
    {
        var result = await ApplicationDbContext.MasterdataTypes
            .ProjectTo<MasterdataTypeModel>(ApplicationService.Mapper.ConfigurationProvider, m => m.Masterdatas).FirstOrDefaultAsync(m => m.Id == id);

        return result;
    }

    public async Task<MasterdataTypeModel?> GetMasterdataType(string code)
    {
        var result = await ApplicationDbContext.MasterdataTypes
            .ProjectTo<MasterdataTypeModel>(ApplicationService.Mapper.ConfigurationProvider, m => m.Masterdatas).FirstOrDefaultAsync(m => m.Code == code);

        return result;
    }

    public async Task<MasterdataTypeListModel> AddMasterdataType(MasterdataTypeCreateModel model)
    {
        var entry = ApplicationDbContext.MasterdataTypes.Add(ApplicationService.Mapper.Map<MasterdataType>(model));
        await ApplicationDbContext.SaveChangesAsync();
        return ApplicationService.Mapper.Map<MasterdataTypeListModel>(entry.Entity);
    }

    public async Task<MasterdataTypeListModel> UpdateMasterdataType(Guid masterdataTypeId, MasterdataTypeUpdateModel model)
    {
        var dbEntity = await ApplicationDbContext.MasterdataTypes.FindAsync(masterdataTypeId);
        if (dbEntity == default)
        {
            throw new NotFoundException($"MasterdataType with id '{masterdataTypeId}' not found.");
        }

        ApplicationService.Mapper.Map(model, dbEntity);
        await ApplicationDbContext.SaveChangesAsync();
        return ApplicationService.Mapper.Map<MasterdataTypeListModel>(dbEntity);
    }


    public async Task<IEnumerable<MasterdataModel>> GetMasterdata(string masterdataType)
    {
        Expression<Func<Entities.Masterdata, bool>> where = null!;
        if (Guid.TryParse(masterdataType, out var masterdataTypeId))
            where = m => m.MasterdataTypeId == masterdataTypeId;
        else
            where = m => m.MasterdataType!.Code == masterdataType;

        var result = await ApplicationDbContext.Masterdatas
            .ProjectTo<MasterdataModel>(ApplicationService.Mapper.ConfigurationProvider).ToListAsync();

        return result;
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
        
        var result = await ApplicationDbContext.Masterdatas
            .Where(whereMasterdataType)
            .Where(whereValue)
            .ProjectTo<MasterdataModel>(ApplicationService.Mapper.ConfigurationProvider).FirstOrDefaultAsync();

        return result;
    }

    public async Task<MasterdataModel> AddMasterdata(MasterdataCreateModel model)
    {
        var result = await base.Add<MasterdataModel, MasterdataCreateModel>(model);
        return result;
    }

    public async Task<MasterdataModel> UpdateMasterdata(Guid masterdataId, MasterdataUpdateModel model)
    {
        var result = await base.Update<MasterdataModel, MasterdataUpdateModel>(masterdataId, model);
        return result;
    }

    public async Task<MasterdataTypeModel?> ImportMasterdata(MasterdataImportModel model)
    {
        var newType = new MasterdataTypeCreateModel
        {
            Name = model.Name,
            Description = model.Description,
            Code = model.Code,
            Metadata = model.Metadata
        };

        var entry = ApplicationDbContext.MasterdataTypes.Add(ApplicationService.Mapper.Map<MasterdataType>(newType));


        foreach (var masterdataModel in model.Masterdatas)
        {
            entry.Entity.Masterdatas.Add(ApplicationService.Mapper.Map<Entities.Masterdata>(masterdataModel));
        }

        await ApplicationDbContext.SaveChangesAsync();

        return await GetMasterdataType(entry.Entity.Id);
    }

    public async Task DeleteMasterdataType(Guid id)
    {
        var entity = await ApplicationDbContext.MasterdataTypes.FindAsync(id);
        if (entity == default)
            return;

        ApplicationDbContext.Remove(entity);
        await ApplicationDbContext.SaveChangesAsync();
    }

    public async Task DeleteMasterdata(Guid id)
    {
        await base.SoftDelete(id);
    }
}
