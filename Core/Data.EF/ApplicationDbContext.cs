using Lens.Core.Data.EF.Entities;
using Lens.Core.Data.EF.Services;
using Lens.Core.Data.Services;
using Lens.Core.Lib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EFCore = Microsoft.EntityFrameworkCore.EF;
using Lens.Core.Data.EF.AuditTrail;

namespace Lens.Core.Data.EF
{
    public class ApplicationDbContext : DbContext
    {   
        private Guid _tenantId;
        private readonly IUserContext _userContext;
        private readonly IAuditTrailService _auditTrailService;
        private readonly IEnumerable<IModelBuilderService> _modelBuilders;

        public ApplicationDbContext(DbContextOptions options,
            IUserContext userContext,
            IAuditTrailService auditTrailService,
            IEnumerable<IModelBuilderService> modelBuilders) : base(options)
        {
            if (userContext != null && userContext.HasClaim("TenantId"))
            {
                _tenantId = userContext.ClaimValue<Guid>("TenantId");
            }

            _userContext = userContext;
            _auditTrailService = auditTrailService;
            _modelBuilders = modelBuilders;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var entities = modelBuilder.Model.GetEntityTypes();
            foreach (var item in entities)
            {
                ConfigureBaseProperties(item.ClrType, modelBuilder);
            }
        }

        protected void ConfigureBaseProperties(Type entityType, ModelBuilder modelBuilder)
        {
            modelBuilder.Entity(entityType, builder =>
            {
                if (!Database.IsSqlServer()) return;
                
                foreach (var service in _modelBuilders)
                {
                    service.ConfigureBaseProperties(entityType, builder);
                }

                SetGlobalQueryFilters(modelBuilder);
            });
        }

        #region Save logic
        public override int SaveChanges()
        {
            throw new Exception("Please use async save method");
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            throw new Exception("Please use async save method");
        }

        // All overloaded saves end up in this save method.
        // Only need AuditTrailing in here.
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            List<EntityState> trackedEntityStates = new() { EntityState.Added, EntityState.Deleted, EntityState.Modified };
            ChangeTracker.Entries()
                .Where(e => trackedEntityStates.Contains(e.State) && e.Entity is ICreatedUpdatedEntity)
                .ToList()
                .ForEach(entry =>
                {
                    entry.Property(ShadowProperties.UpdatedOn).CurrentValue = DateTime.Now;
                    entry.Property(ShadowProperties.UpdatedBy).CurrentValue = _userContext.Username;
                });

            var changes = this.CaptureChanges(_userContext).ToList();


            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            changes
                .Where(c => c.ChangeType == EntityState.Added.ToString())
                .ToList()
                .ForEach(c => c.ResolveId(c));

            await _auditTrailService.LogChanges(() => changes);
            return result;
        }

        #endregion Save logic

        private void SetGlobalQueryFilters(ModelBuilder modelBuilder)
        {
            foreach (var tp in modelBuilder.Model.GetEntityTypes())
            {
                var t = tp.ClrType;

                // set global filters
                if (typeof(ITenantEntity).IsAssignableFrom(t))
                {
                    // note do not filter just ITenant - too much filtering! 
                    // just top level classes that have ITenantEntity
                    var method = SetGlobalQueryForTenantMethodInfo.MakeGenericMethod(t);
                    method.Invoke(this, new object[] { modelBuilder });
                }
            }
        }

        private static readonly MethodInfo SetGlobalQueryForTenantMethodInfo = typeof(ApplicationDbContext).GetMethods(BindingFlags.Public | BindingFlags.Instance)
           .Single(t => t.IsGenericMethod && t.Name == "SetGlobalQueryForTenant");


        /// TODO: an elegant and safe solution is to combine the base query filter with the tenant filter
        public void SetGlobalQueryForTenant<T>(ModelBuilder builder) where T : class, ITenantEntity
        {
            //the HasQueryFilter applies only the last query; so, in this case overrides the check for deleted records from the base package.
            builder.Entity<T>().HasQueryFilter(item =>
                EFCore.Property<RecordStateEnum>(item, ShadowProperties.RecordState) != RecordStateEnum.Deleted
                && EFCore.Property<Guid>(item, ShadowProperties.TenantId) == _tenantId);
        }
    }
}
