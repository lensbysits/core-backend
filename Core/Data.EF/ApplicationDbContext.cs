using Lens.Core.Data.EF.AuditTrail;
using Lens.Core.Data.EF.Entities;
using Lens.Core.Data.EF.Services;
using Lens.Core.Data.EF.Translation.Attributes;
using Lens.Core.Data.EF.Translation.Models;
using Lens.Core.Data.Services;
using Lens.Core.Lib.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;
using EFCore = Microsoft.EntityFrameworkCore.EF;

namespace Lens.Core.Data.EF;

public class ApplicationDbContext : DbContext
{   
    private readonly Guid _tenantId;
    private readonly IUserContext? _userContext;
    private readonly IAuditTrailService? _auditTrailService;
    private readonly IEnumerable<IModelBuilderService> _modelBuilders;
    private static readonly MethodInfo SetGlobalQueryForTenantMethodInfo = 
        typeof(ApplicationDbContext).GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Single(t => t.IsGenericMethod && t.Name == "SetGlobalQueryForTenant");

    public ApplicationDbContext(DbContextOptions options,
        IUserContext userContext,
        IAuditTrailService auditTrailService,
        IEnumerable<IModelBuilderService> modelBuilders) : this(options, userContext, modelBuilders)
    {
        _auditTrailService = auditTrailService;
    }

    public ApplicationDbContext(DbContextOptions options,
        IUserContext userContext,
        IEnumerable<IModelBuilderService> modelBuilders) : base(options)
    {
        if (userContext != null && userContext.HasClaim("TenantId"))
        {
            _tenantId = userContext.ClaimValue<Guid>("TenantId");
        }

        _userContext = userContext;
        _modelBuilders = modelBuilders;
    }

    #region Protected methods
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
            // TODO: JST - 16-08-2022 - Check if indeed also works on other databases. In that case, this check is not needed
            // If this does not work for other databases, this method should move to a database-specific library.
            //if (!Database.IsSqlServer()) return;
            
            foreach (var service in _modelBuilders)
            {
                service.ConfigureBaseProperties(entityType, builder);
            }

            SetGlobalQueryFilters(modelBuilder);
        });
    }
    #endregion Protected methods

    #region Public methods
    /// TODO: an elegant and safe solution is to combine the base query filter with the tenant filter
    public void SetGlobalQueryForTenant<T>(ModelBuilder builder) where T : class, ITenantEntity
    {
        //the HasQueryFilter applies only the last query; so, in this case overrides the check for deleted records from the base package.
        builder.Entity<T>().HasQueryFilter(item =>
            EFCore.Property<RecordStateEnum>(item, ShadowProperties.RecordState) != RecordStateEnum.Deleted
            && EFCore.Property<Guid>(item, ShadowProperties.TenantId) == _tenantId);
    }

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
        SetCreatedUpdatedFields();
        InitializeTranslation();

        var changes = this.CaptureChanges(_userContext).ToList();

        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        changes
            .Where(c => c.ChangeType == EntityState.Added.ToString())
            .ToList()
            .ForEach(c => { if (c.ResolveId != null) c.ResolveId(c); });

        if (_auditTrailService != null)
        {
            await _auditTrailService.LogChanges(() => changes);
        }

        return result;
    }
    #endregion Public methods

    #region Private methods
    private void SetCreatedUpdatedFields()
    {
        List<EntityState> trackedEntityStates = new() { EntityState.Added, EntityState.Deleted, EntityState.Modified };
        
        // setup Updated fields
        ChangeTracker.Entries()
            .Where(e => trackedEntityStates.Contains(e.State) && e.Entity is ICreatedUpdatedEntity)
            .ToList()
            .ForEach(entry =>
            {
                entry.Property(ShadowProperties.UpdatedOn).CurrentValue = DateTime.UtcNow;
                entry.Property(ShadowProperties.UpdatedBy).CurrentValue = _userContext?.Username;
            });

        // setup Created fields
        ChangeTracker.Entries()
           .Where(e => e.State == EntityState.Added && e.Entity is ICreatedUpdatedEntity)
           .ToList()
           .ForEach(entry =>
           {
               entry.Property(ShadowProperties.CreatedBy).CurrentValue = _userContext?.Username;
           });
    }

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

    private void InitializeTranslation()
    {
        ChangeTracker.DetectChanges();

        var addedTranslationEntities = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added && e.Entity is ITranslationEntity);

        foreach (var entry in addedTranslationEntities)
        {
            var translationModel = new TranslationModel("en-US", true);
            foreach (var property in entry.Properties)
            {
                var hasTranslatableAttribute =
                    property.Metadata.PropertyInfo != null
                    && property.Metadata.PropertyInfo.GetCustomAttributes(typeof(TranslatableAttribute), false).Any();

                if (hasTranslatableAttribute)
                {
                    translationModel.Values.Add(
                        new TranslatedField(property.Metadata.Name));
                }
            }

            var result = new TranslationModel[]
            {
                translationModel
            };
            entry.Property(ShadowProperties.Translation).CurrentValue = JsonSerializer.Serialize(result);
        }
    }
    #endregion
}
