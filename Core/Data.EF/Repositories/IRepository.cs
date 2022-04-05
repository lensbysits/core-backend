using Lens.Core.Data.EF.Entities;
using Lens.Core.Data.Repositories;

namespace Lens.Core.Data.EF.Repositories;


public interface IRepository<TDbContext> : IRepository
    where TDbContext : ApplicationDbContext
{
}

public interface IRepository<TDbContext, TEntity> : IRepository<TDbContext>
    where TDbContext : ApplicationDbContext
    where TEntity : class, IIdEntity
{
}
