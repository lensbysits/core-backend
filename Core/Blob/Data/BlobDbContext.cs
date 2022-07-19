using Lens.Core.Blob.Data.Entities;
using Lens.Core.Data.EF;
using Lens.Core.Data.EF.Services;
using Lens.Core.Data.Services;
using Lens.Core.Lib.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Lens.Core.Blob.Data
{
    public class BlobDbContext : ApplicationDbContext
    {
        public BlobDbContext(DbContextOptions<BlobDbContext> options,
            IUserContext userContext,
            IAuditTrailService auditTrailService,
            IEnumerable<IModelBuilderService> modelBuilders) : base(options, userContext, auditTrailService, modelBuilders)
        {
        }

        public DbSet<BlobInfo> BlobInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureBaseProperties(typeof(BlobInfo), modelBuilder);
        }
    }
}
