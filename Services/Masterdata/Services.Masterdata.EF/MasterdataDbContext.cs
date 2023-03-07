using Lens.Core.Data.EF;
using Lens.Core.Data.EF.Services;
using Lens.Core.Lib.Services;
using Lens.Services.Masterdata.EF.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lens.Services.Masterdata.EF;

public class MasterdataDbContext : ApplicationDbContext
{
    public MasterdataDbContext(DbContextOptions<MasterdataDbContext> options, IUserContext userContext, IEnumerable<IModelBuilderService> modelBuilders) : base(options, userContext, modelBuilders)
    {
    }

    public virtual DbSet<MasterdataType> MasterdataTypes { get; set; } = null!;
    public virtual DbSet<Entities.Masterdata> Masterdatas { get; set; } = null!;
    public virtual DbSet<Entities.MasterdataKey> MasterdataKeys { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MasterdataType>()
            .HasIndex(m => m.Code).IsUnique();

        modelBuilder.Entity<Entities.Masterdata>()
            .HasIndex(m => new { m.MasterdataTypeId, m.Key }).IsUnique();

        modelBuilder.Entity<Entities.MasterdataKey>()
            .HasIndex(m => new { m.MasterdataId, m.Domain, m.Key }).IsUnique();
    }
}
