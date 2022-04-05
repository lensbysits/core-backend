using AutoMapper.QueryableExtensions;
using Lens.Core.Data.EF.Entities;
using Lens.Core.Lib.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lens.Core.Data.EF.Repositories;

public class Repository<TDbContext> : IRepository<TDbContext> where TDbContext : ApplicationDbContext
{
    public Repository(TDbContext dbContext, IApplicationService<Repository<TDbContext>> applicationService)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        ApplicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
    }

    protected TDbContext DbContext { get; }
    protected IApplicationService<Repository<TDbContext>> ApplicationService { get; }

    public async Task<TEntity> Create<TEntity>(TEntity entity)
    {
        await DbContext.AddAsync(entity);
        await DbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<TEntity>> Create<TEntity>(IEnumerable<TEntity> entities)
    {
        await DbContext.AddRangeAsync(entities);
        await DbContext.SaveChangesAsync();
        return entities;
    }
}

public class Repository<TDbContext, TEntity> : Repository<TDbContext>, IRepository<TDbContext, TEntity>
    where TDbContext : ApplicationDbContext
    where TEntity : class, IIdEntity
{
    public Repository(TDbContext dbContext, IApplicationService<Repository<TDbContext>> applicationService) : base(dbContext, applicationService)
    {
    }

    public async Task<TResult> CreateFromModel<TResult>(object model)
    {
        var entity = ApplicationService.Mapper.Map<TEntity>(model);
        entity = await Create(entity);
        var result = ApplicationService.Mapper.Map<TResult>(entity);
        return result;
    }

    public async Task<TModel> Get<TModel>(Guid id)
    {
        var result = await DbContext.Set<TEntity>()
                                    .Where(entity => entity.Id == id)
                                    .ProjectTo<TModel>(ApplicationService.Mapper.ConfigurationProvider)
                                    .FirstOrDefaultAsync();
        return result;
    }

    public async Task<IEnumerable<TModel>> Get<TModel>(Expression<Func<TEntity, int, bool>> predicate = null)
    {
        IQueryable<TEntity> query = DbContext.Set<TEntity>();
        if(predicate != null)
            query.Where(predicate);
        
        var result = await query
                        .ProjectTo<TModel>(ApplicationService.Mapper.ConfigurationProvider)
                        .ToListAsync();

        return result;
    }
}
