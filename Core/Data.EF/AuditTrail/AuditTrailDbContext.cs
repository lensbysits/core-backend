using Lens.Core.Data.EF.AuditTrail.Entities;
using Lens.Core.Data.EF.Services;
using Lens.Core.Lib.Services;
using Microsoft.EntityFrameworkCore;

namespace Lens.Core.Data.EF.AuditTrail;

public class AuditTrailDbContext : ApplicationDbContext
{
    public AuditTrailDbContext(DbContextOptions<AuditTrailDbContext> options, IUserContext userContext, IEnumerable<IModelBuilderService> modelBuilders) : base(options, userContext, modelBuilders)
    {
    }

    public DbSet<EntityChange> EntityChanges { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
