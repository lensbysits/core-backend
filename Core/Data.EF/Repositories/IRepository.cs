using Lens.Core.Data.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lens.Core.Data.EF.Repositories;

public interface IRepository<TDbContext> where TDbContext : ApplicationDbContext
{
    Task<TEntity> Create<TEntity>(TEntity entity);
    Task<IEnumerable<TEntity>> Create<TEntity>(IEnumerable<TEntity> entities);
}

public interface IRepository<TDbContext, TEntity> : IRepository<TDbContext>
    where TDbContext : ApplicationDbContext
    where TEntity : class, IIdEntity
{
    Task<TModel> Get<TModel>(Guid id);
    Task<IEnumerable<TModel>> Get<TModel>(Expression<Func<TEntity, int, bool>> predicate = null);

    Task<TResult> CreateFromModel<TResult>(object model);
}
