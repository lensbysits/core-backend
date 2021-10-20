﻿using Lens.Core.Data.EF.Entities;
using Lens.Core.Lib.Models;
using Lens.Core.Lib.Services;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Lens.Core.Data.EF.Services
{
    public class DataService<TService, T, TDbContext> : BaseService<TService>
        where T : class, IIdEntity
        where TDbContext : ApplicationDbContext
    {
        protected TDbContext ApplicationDbContext { get; set; }

        public DataService(
            IApplicationService<TService> applicationService,
            TDbContext applicationDbContext) : base(applicationService)
        {
            ApplicationDbContext = applicationDbContext;
        }

        protected async Task<ResultListModel<TModel>> Get<TModel>(QueryModel queryModel, Expression<Func<T, bool>> searchPredicate = null, Expression<Func<T, bool>> filterPredicate = null)
        {
            var entities = ApplicationDbContext.Set<T>().AsQueryable();
            if (filterPredicate != null)
            {
                entities = entities.Where(filterPredicate);
            }

            return await entities
                   .GetByQueryModel(queryModel, searchPredicate)
                   .ToResultList<T, TModel>(queryModel, ApplicationService.Mapper);
        }

        protected async Task<TModel> Get<TModel>(Guid id)
        {
            return await ApplicationDbContext.Set<T>()
                .ToModel<T, TModel>(id, ApplicationService.Mapper);
        }

        protected async Task<TModel> Add<TModel, VModel>(VModel value)
        {
            var entity = ApplicationService.Mapper.Map<T>(value);
            var trackedEntity = ApplicationDbContext.Set<T>().Add(entity).Entity;
            
            await ApplicationDbContext.SaveChangesAsync();

            return await Get<TModel>(trackedEntity.Id);
        }

        protected async Task<TModel> Update<TModel, VModel>(Guid id, VModel value)
        {
            var entity = await ApplicationDbContext.Set<T>().GetById(id);
            ApplicationService.Mapper.Map(value, entity);
            ApplicationDbContext.Set<T>().Update(entity);

            await ApplicationDbContext.SaveChangesAsync();

            return await Get<TModel>(id);
        }

        protected async Task SoftDelete(Guid id)
        {
            var entity = await ApplicationDbContext.Set<T>().GetById(id);

            ApplicationDbContext.Entry(entity).Property(ShadowProperties.RecordState).CurrentValue = RecordStateEnum.Deleted;
            ApplicationDbContext.Set<T>().Update(entity);

            await ApplicationDbContext.SaveChangesAsync();
        }

        protected async Task HardDelete(Guid id)
        {
            var entity = await ApplicationDbContext.Set<T>().GetById(id);
            ApplicationDbContext.Set<T>().Remove(entity);

            await ApplicationDbContext.SaveChangesAsync();
        }
    }
}