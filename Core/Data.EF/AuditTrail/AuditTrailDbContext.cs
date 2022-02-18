using Lens.Core.Data.EF.AuditTrail.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lens.Core.Data.EF.AuditTrail
{
    public class AuditTrailDbContext : DbContext
    {
        public AuditTrailDbContext(DbContextOptions<AuditTrailDbContext> options) : base(options)
        {
        }

        public DbSet<EntityChange> EntityChanges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EntityChange>(builder =>
            {
                if (!Database.IsSqlServer()) return;

                builder
                    .Property(e => e.Id)
                    .HasDefaultValueSql("newsequentialid()")
                    .ValueGeneratedOnAdd();
            });
        }
    }
}
