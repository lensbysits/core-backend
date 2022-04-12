using Lens.Core.Blob.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lens.Core.Blob.Data
{
    public class BlobDbContext : DbContext
    {
        public BlobDbContext(DbContextOptions<BlobDbContext> options) : base(options)
        {
        }

        public DbSet<BlobInfo> BlobInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlobInfo>(builder =>
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
