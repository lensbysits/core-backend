using AutoMapper.QueryableExtensions;
using Lens.Core.Lib.Exceptions;
using Lens.Core.Lib.Services;
using Lens.Services.Masterdata.EF.Entities;
using Lens.Services.Masterdata.Models;
using Lens.Services.Masterdata.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Lens.Services.Masterdata.EF.Services;

public class MasterdataService : BaseService<MasterdataService>, IMasterdataService
{
    private readonly MasterdataDbContext _masterdataDbContext;

    public MasterdataService(MasterdataDbContext masterdataDbContext, IApplicationService<MasterdataService> applicationService) : base(applicationService)
    {
        _masterdataDbContext = masterdataDbContext;
    }

    public async Task<IEnumerable<MasterdataTypeListBM>> GetMasterdataTypes()
    {
        var result = await _masterdataDbContext.MasterdataTypes
            .ProjectTo<MasterdataTypeListBM>(ApplicationService.Mapper.ConfigurationProvider).ToListAsync();

        return result;
    }

    public async Task<MasterdataTypeBM?> GetMasterdataType(Guid id)
    {
        var result = await _masterdataDbContext.MasterdataTypes
            .ProjectTo<MasterdataTypeBM>(ApplicationService.Mapper.ConfigurationProvider, m => m.Masterdatas).FirstOrDefaultAsync(m => m.Id == id);

        return result;
    }

    public async Task<MasterdataTypeBM?> GetMasterdataType(string code)
    {
        var result = await _masterdataDbContext.MasterdataTypes
            .ProjectTo<MasterdataTypeBM>(ApplicationService.Mapper.ConfigurationProvider, m => m.Masterdatas).FirstOrDefaultAsync(m => m.Code == code);

        return result;
    }

    public async Task<MasterdataTypeListBM> AddMasterdataType(MasterdataTypeCreateBM model)
    {
        var entry = _masterdataDbContext.MasterdataTypes.Add(ApplicationService.Mapper.Map<MasterdataType>(model));
        await _masterdataDbContext.SaveChangesAsync();
        return ApplicationService.Mapper.Map<MasterdataTypeListBM>(entry.Entity);
    }

    public async Task<MasterdataTypeListBM> UpdateMasterdataType(Guid masterdataTypeId, MasterdataTypeUpdateBM model)
    {
        var dbEntity = await _masterdataDbContext.MasterdataTypes.FindAsync(masterdataTypeId);
        if (dbEntity == default)
        {
            throw new NotFoundException($"MasterdataType with id '{masterdataTypeId}' not found.");
        }

        ApplicationService.Mapper.Map(model, dbEntity);
        await _masterdataDbContext.SaveChangesAsync();
        return ApplicationService.Mapper.Map<MasterdataTypeListBM>(dbEntity);
    }


    public async Task<IEnumerable<MasterdataBM>> GetMasterdata(string masterdataType)
    {
        Expression<Func<Entities.Masterdata, bool>> where = null!;
        if (Guid.TryParse(masterdataType, out var masterdataTypeId))
            where = m => m.MasterdataTypeId == masterdataTypeId;
        else
            where = m => m.MasterdataType!.Code == masterdataType;

        var result = await _masterdataDbContext.Masterdatas
            .ProjectTo<MasterdataBM>(ApplicationService.Mapper.ConfigurationProvider).ToListAsync();

        return result;
    }

    public async Task<MasterdataBM?> GetMasterdata(string masterdataType, string value)
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

        var result = await _masterdataDbContext.Masterdatas
            .Where(whereMasterdataType)
            .Where(whereValue)
            .ProjectTo<MasterdataBM>(ApplicationService.Mapper.ConfigurationProvider).FirstOrDefaultAsync();

        return result;
    }

    public async Task<MasterdataBM> AddMasterdata(MasterdataCreateBM model)
    {
        var entry = _masterdataDbContext.Masterdatas.Add(ApplicationService.Mapper.Map<Entities.Masterdata>(model));
        await _masterdataDbContext.SaveChangesAsync();
        return ApplicationService.Mapper.Map<MasterdataBM>(entry.Entity);
    }

    public async Task<MasterdataBM> UpdateMasterdata(Guid masterdataId, MasterdataUpdateBM model)
    {
        var dbEntity = await _masterdataDbContext.Masterdatas.FindAsync(masterdataId);
        if (dbEntity == default)
        {
            throw new NotFoundException($"Masterdata with id '{masterdataId}' not found.");
        }

        ApplicationService.Mapper.Map(model, dbEntity);
        await _masterdataDbContext.SaveChangesAsync();
        return ApplicationService.Mapper.Map<MasterdataBM>(dbEntity);
    }

    public async Task<MasterdataTypeBM?> ImportMasterdata(MasterdataImportBM model)
    {
        var newType = new MasterdataTypeCreateBM
        {
            Name = model.Name,
            Description = model.Description,
            Code = model.Code,
            Metadata = model.Metadata
        };

        var entry = _masterdataDbContext.MasterdataTypes.Add(ApplicationService.Mapper.Map<MasterdataType>(newType));


        foreach (var masterdataModel in model.Masterdatas)
        {
            entry.Entity.Masterdatas.Add(ApplicationService.Mapper.Map<Entities.Masterdata>(masterdataModel));
        }

        await _masterdataDbContext.SaveChangesAsync();

        return await GetMasterdataType(entry.Entity.Id);
    }

    public async Task DeleteMasterdataType(Guid id)
    {
        var entity = await _masterdataDbContext.MasterdataTypes.FindAsync(id);
        if (entity == default)
            return;

        _masterdataDbContext.Remove(entity);
        await _masterdataDbContext.SaveChangesAsync();
    }

    public async Task DeleteMasterdata(Guid id)
    {
        var entity = await _masterdataDbContext.Masterdatas.FindAsync(id);
        if (entity == default)
            return;

        _masterdataDbContext.Remove(entity);
        await _masterdataDbContext.SaveChangesAsync();
    }
}
