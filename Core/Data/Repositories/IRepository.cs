using System.Linq.Expressions;

namespace Lens.Core.Data.Repositories;

public interface IRepository
{
    Task<TResult> Create<TResult>(object model);
    Task<IEnumerable<TResult>> Create<TResult>(IEnumerable<object> models);
    Task<TModel> Get<TModel>(Guid id);
    Task<IEnumerable<TModel>> Get<TModel>(Expression<Func<TModel, int, bool>>? predicate = null);
}
