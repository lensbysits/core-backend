using AutoMapper;
using AutoMapper.QueryableExtensions;
using Lens.Core.Data.EF.Entities;
using Lens.Core.Lib.Exceptions;
using Lens.Core.Lib.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Lens.Core.Data.EF;

public static class EntityToModelExtensions
{
    public async static Task<TModel> ToModel<TEntity, TModel>(this IQueryable<TEntity> entities, Guid id, IMapper mapper)
        where TEntity : class, IIdEntity
    {
        var result = await entities
            .Where(t => t.Id == id)
            .ProjectTo<TModel>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (result == null)
        {
            throw new NotFoundException($"{typeof(TModel).Name} with id {id} not found");
        }

        return result;
    }

    public static async Task<IEnumerable<TModel>> ToModel<TEntity, TModel>(this IQueryable<TEntity> entities, IMapper mapper, Expression<Func<TEntity, bool>>? predicate = null)
        where TEntity : class, IEntity
    {
        if (predicate != null)
        {
            entities = entities.Where(predicate);
        }

        return await entities
            .ProjectTo<TModel>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public static async Task<ResultPagedListModel<TModel>> ToResultList<TEntity, TModel>(this IQueryable<TEntity> entities, QueryModel queryModel, IMapper mapper)
        where TEntity : class, IEntity
    {
        var resultQuery = entities.ProjectTo<TModel>(mapper.ConfigurationProvider);
        var resultList = queryModel.NoLimit
                ? await resultQuery.ToListAsync()
                : await resultQuery.Skip(queryModel.Offset).Take(queryModel.Limit).ToListAsync();
        var resultTotalSize = await resultQuery.CountAsync();
        var result = new ResultPagedListModel<TModel>(resultList)
        {
            TotalSize = resultTotalSize
        };

        return result;
    }
}