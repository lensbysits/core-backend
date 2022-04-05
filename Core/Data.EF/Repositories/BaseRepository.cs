using AutoMapper.QueryableExtensions;
using Lens.Core.Data.EF.Entities;
using Lens.Core.Data.Repositories;
using Lens.Core.Lib.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lens.Core.Data.EF.Repositories;

public abstract class BaseRepository<TDbContext> : IRepository<TDbContext>
    where TDbContext : ApplicationDbContext
{
    public BaseRepository(TDbContext dbContext, IApplicationService<BaseRepository<TDbContext>> applicationService) : base()
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        ApplicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
    }

    protected TDbContext DbContext { get; }
    protected IApplicationService<BaseRepository<TDbContext>> ApplicationService { get; }

    public virtual Task<TResult> Create<TResult>(object model) { return default; }

    public virtual Task<IEnumerable<TResult>> Create<TResult>(IEnumerable<object> models) { return default; }

    public virtual Task<TModel> Get<TModel>(Guid id) { return default; }

    public virtual Task<IEnumerable<TModel>> Get<TModel>(Expression<Func<TModel, int, bool>> predicate = null) { return default; }
}

public abstract class BaseRepository<TDbContext, TEntity>: BaseRepository<TDbContext>, IRepository
    where TDbContext : ApplicationDbContext
    where TEntity : class, IIdEntity
{
    public BaseRepository(TDbContext dbContext, IApplicationService<BaseRepository<TDbContext>> applicationService) : base(dbContext, applicationService)
    {
    }

    public override async Task<TResult> Create<TResult>(object model)
    {
        var entity = ApplicationService.Mapper.Map<TEntity>(model);
        await DbContext.AddAsync(entity);
        await DbContext.SaveChangesAsync();
        var result = ApplicationService.Mapper.Map<TResult>(entity);
        return result;
    }

    public override async Task<IEnumerable<TResult>> Create<TResult>(IEnumerable<object> models)
    {
        var entities = models.Select(model => ApplicationService.Mapper.Map<TEntity>(model)).ToList();
        await DbContext.AddRangeAsync(entities);
        await DbContext.SaveChangesAsync();
        return entities.Select(entity => ApplicationService.Mapper.Map<TResult>(entity)).ToList();
    }

    public override async Task<TModel> Get<TModel>(Guid id)
    {
        var result = await DbContext.Set<TEntity>()
                                    .Where(entity => entity.Id == id)
                                    .ProjectTo<TModel>(ApplicationService.Mapper.ConfigurationProvider)
                                    .FirstOrDefaultAsync();
        return result;
    }

    public override async Task<IEnumerable<TModel>> Get<TModel>(Expression<Func<TModel, int, bool>> predicate = null)
    {
        IQueryable<TModel> query = DbContext.Set<TEntity>()
            .ProjectTo<TModel>(ApplicationService.Mapper.ConfigurationProvider);

        if(predicate != null)
            query.Where(predicate);
        
        var result = await query.ToListAsync();

        return result;
    }
}
