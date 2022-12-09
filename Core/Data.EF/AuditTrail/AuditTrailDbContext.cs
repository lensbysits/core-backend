using Lens.Core.Data.EF.AuditTrail.Entities;
using Lens.Core.Data.EF.AuditTrail.RowLevelSecurity.Providers;
using Lens.Core.Data.EF.Configuration;
using Lens.Core.Data.EF.RowLevelSecurity.Interceptors;
using Lens.Core.Data.EF.Services;
using Lens.Core.Lib.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Lens.Core.Data.EF.AuditTrail;

public class AuditTrailDbContext : ApplicationDbContext
{
    private readonly IAuditRlsIdentityProvider? rlsIdentityProvider;
    private readonly AuditSettings auditSettings;

    public AuditTrailDbContext(
        DbContextOptions<AuditTrailDbContext> options,
        IUserContext userContext,
        IEnumerable<IModelBuilderService> modelBuilders,
        IOptions<AuditSettings> auditSettings,
        IAuditRlsIdentityProvider? rlsIdentityProvider)
        : base(options, userContext, modelBuilders)
    {
        this.rlsIdentityProvider = rlsIdentityProvider;
        this.auditSettings = auditSettings.Value;
    }

    public DbSet<EntityChange> EntityChanges { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (this.auditSettings.EnableRlsSupport)
        {
            optionsBuilder.AddInterceptors(new RowLevelSecurityInterceptor(rlsIdentityProvider));
        }

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
