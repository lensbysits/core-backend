using Lens.Core.Data.EF.Entities;
using Lens.Core.Lib;
using Lens.Core.Lib.Models;
using Lens.Core.Lib.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Lens.Core.Data.EF.Services
{
    public abstract class DataService<TService, TEntity, TDbContext> : BaseService<TService>
        where TEntity : class, IIdEntity
        where TDbContext : ApplicationDbContext
    {
        protected TDbContext ApplicationDbContext { get; set; }

        public DataService(
            IApplicationService<TService> applicationService,
            TDbContext applicationDbContext) : base(applicationService)
        {
            ApplicationDbContext = applicationDbContext;
        }

        protected async Task<ResultListModel<TModel>> Get<TModel>(QueryModel queryModel, Expression<Func<TEntity, bool>> searchPredicate = null, Expression<Func<TEntity, bool>> filterPredicate = null)
        {
            var entities = ApplicationDbContext.Set<TEntity>().AsQueryable();
            if (filterPredicate != null)
            {
                entities = entities.Where(filterPredicate);
            }

            return await entities
                   .GetByQueryModel(queryModel, searchPredicate)
                   .ToResultList<TEntity, TModel>(queryModel, ApplicationService.Mapper);
        }

        protected async Task<TModel> Get<TModel>(Guid id)
        {
            return await ApplicationDbContext.Set<TEntity>()
                .ToModel<TEntity, TModel>(id, ApplicationService.Mapper);
        }

        protected async Task<IEnumerable<TModel>> Get<TModel>(Expression<Func<TEntity, bool>> filterPredicate = null)
        {
            return await ApplicationDbContext.Set<TEntity>()
                .ToModel<TEntity, TModel>(ApplicationService.Mapper, filterPredicate);
        }

        protected async Task<TModel> Add<TModel, VModel>(VModel value)
        {
            var entity = ApplicationService.Mapper.Map<TEntity>(value);
            var trackedEntity = ApplicationDbContext.Set<TEntity>().Add(entity).Entity;
            
            await ApplicationDbContext.SaveChangesAsync();
            ApplicationService.Logger.LogInformation(LoggingEvents.InsertItem, $"Added {typeof(TEntity).Name} with id '{trackedEntity.Id}'");

            return await Get<TModel>(trackedEntity.Id);
        }

        protected async Task<TModel> Update<TModel, VModel>(Guid id, VModel value)
        {
            var entity = await ApplicationDbContext.Set<TEntity>().GetById(id);
            ApplicationService.Mapper.Map(value, entity);
            ApplicationDbContext.Set<TEntity>().Update(entity);

            await ApplicationDbContext.SaveChangesAsync();
            ApplicationService.Logger.LogDebug(LoggingEvents.UpdateItem, $"Updated {typeof(TEntity).Name} with id '{id}'");

            return await Get<TModel>(id);
        }

        protected async Task SoftDelete(Guid id)
        {
            var entity = await ApplicationDbContext.Set<TEntity>().GetById(id);

            ApplicationDbContext.Entry(entity).Property(ShadowProperties.RecordState).CurrentValue = RecordStateEnum.Deleted;
            ApplicationDbContext.Set<TEntity>().Update(entity);

            await ApplicationDbContext.SaveChangesAsync();
            ApplicationService.Logger.LogInformation(LoggingEvents.DeleteItem, $"Soft deleted {typeof(TEntity).Name} with id '{id}'");
        }

        protected async Task HardDelete(Guid id)
        {
            var entity = await ApplicationDbContext.Set<TEntity>().GetById(id);
            ApplicationDbContext.Set<TEntity>().Remove(entity);

            await ApplicationDbContext.SaveChangesAsync();
            ApplicationService.Logger.LogInformation(LoggingEvents.DeleteItem, $"Hard deleted {typeof(TEntity).Name} with id '{id}'");
        }

        protected async Task HardDelete(Expression<Func<TEntity, bool>> filterPredicate)
        {
            ApplicationDbContext.Set<TEntity>().DeleteWhere(filterPredicate);
            
            await ApplicationDbContext.SaveChangesAsync();
            ApplicationService.Logger.LogInformation(LoggingEvents.DeleteItem, $"Hard deleted multipe entities of type {typeof(TEntity).Name}");
        }
    }
}