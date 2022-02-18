using AutoMapper;
using AutoMapper.QueryableExtensions;
using Lens.Core.Data.EF.Entities;
using Lens.Core.Lib.Exceptions;
using Lens.Core.Lib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lens.Core.Data.EF
{
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

        public static async Task<IEnumerable<TModel>> ToModel<TEntity, TModel>(this IQueryable<TEntity> entities, IMapper mapper, Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class, IIdEntity
        {
            if (predicate != null)
            {
                entities = entities.Where(predicate);
            }

            return await entities
                .ProjectTo<TModel>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public static async Task<ResultListModel<TModel>> ToResultList<TEntity, TModel>(this IQueryable<TEntity> entities, QueryModel queryModel, IMapper mapper)
            where TEntity : class, IIdEntity
        {
            var result = entities.ProjectTo<TModel>(mapper.ConfigurationProvider);

            return new ResultListModel<TModel>
            {
                Value = queryModel.NoLimit
                    ? await result.Skip(queryModel.Offset).ToListAsync()
                    : await result.Skip(queryModel.Offset).Take(queryModel.Limit).ToListAsync(),
                Size = await result.CountAsync()
            };
        }
    }
}