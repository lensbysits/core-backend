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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EFCore = Microsoft.EntityFrameworkCore.EF;

namespace Lens.Core.Data.EF
{
    public static class DataServiceExtension
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

        public static async Task<IEnumerable<TModel>> ToModel<TEntity, TModel>(this IQueryable<TEntity> entities, IMapper mapper)
            where TEntity : class, IIdEntity
        {
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
                Value = await result.Skip(queryModel.Offset).Take(queryModel.Limit).ToListAsync(),
                Size = await result.CountAsync()
            };
        }

        public static async Task<TEntity> GetById<TEntity>(this IQueryable<TEntity> entities, Guid id)
            where TEntity : class, IIdEntity
        {
            TEntity result = await entities.FirstOrDefaultAsync(entity => entity.Id == id);
            if (result == null)
            {
                throw new NotFoundException($"{typeof(TEntity).Name} with id {id} not found");
            }
            return result;
        }

        public static IQueryable<TEntity> GetByQueryModel<TEntity>(this IQueryable<TEntity> entities, QueryModel queryModel, Expression<Func<TEntity, bool>> searchPredicate = null)
           where TEntity : class, IIdEntity
        {
            // apply default filters
            if (typeof(ITagsEntity).IsAssignableFrom(typeof(TEntity)) && !string.IsNullOrEmpty(queryModel.Tag))
            {
                entities = entities
                    .Where(entity => EFCore.Property<string>(entity, ShadowProperties.Tag).Contains($"\"{queryModel.Tag}\""));
            }

            if (typeof(ICreatedUpdatedEntity).IsAssignableFrom(typeof(TEntity)))
            {
                if (!string.IsNullOrEmpty(queryModel.CreatedBy))
                {
                    entities = entities
                        .Where(entity => EFCore.Property<string>(entity, ShadowProperties.CreatedBy) == queryModel.CreatedBy);
                }

                if (!string.IsNullOrEmpty(queryModel.UpdatedBy))
                {
                    entities = entities
                        .Where(entity => EFCore.Property<string>(entity, ShadowProperties.UpdatedBy) == queryModel.UpdatedBy);
                }

                if (queryModel.CreatedSince.HasValue)
                {
                    entities = entities
                        .Where(entity => EFCore.Property<DateTime>(entity, ShadowProperties.CreatedOn) >= queryModel.CreatedSince);
                }

                if (queryModel.UpdatedSince.HasValue)
                {
                    entities = entities
                        .Where(entity => EFCore.Property<DateTime>(entity, ShadowProperties.UpdatedOn) >= queryModel.UpdatedSince);
                }
            }

            // search by term
            if (searchPredicate != null && !string.IsNullOrWhiteSpace(queryModel.SearchTerm))
            {
                entities = entities.Where(searchPredicate);
            }

            // apply sorting
            entities = ApplySort(entities, queryModel);

            return entities;
        }

        #region Private static methods

        private static IQueryable<TEntity> ApplySort<TEntity>(IQueryable<TEntity> entities, QueryModel queryModel)
        {
            if (!entities.Any()) return entities;

            // default sorting when 'order by' query param is missing.
            if (string.IsNullOrWhiteSpace(queryModel.OrderBy) && typeof(ICreatedUpdatedEntity).IsAssignableFrom(typeof(TEntity)))
            {
                entities.OrderByDescending(entity => EFCore.Property<DateTime>(entity, ShadowProperties.UpdatedOn));
                return entities;
            }

            var orderParams = queryModel.OrderBy.Split(',', StringSplitOptions.TrimEntries);
            var propertyInfos = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var orderQueryBuilder = new StringBuilder();

            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param)) continue;

                var propertyFromQueryName = param.Split(" ")[0];
                var objectProperty = propertyInfos
                    .FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

                if (objectProperty == null) continue;

                var sortingOrder = param.EndsWith(" desc") ? "descending" : "ascending";
                orderQueryBuilder.Append($"{objectProperty.Name} {sortingOrder}, ");
            }

            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');

            return string.IsNullOrWhiteSpace(orderQuery) ? entities : entities.OrderBy(orderQuery);
        }

        #endregion Private static methods
    }
}