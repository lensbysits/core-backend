using Lens.Core.Data.EF.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Lens.Core.Data.EF.Services
{
    public class TenantModelBuilderService : IModelBuilderService
    {
        public void ConfigureBaseProperties(Type entityType, EntityTypeBuilder builder)
        {
            if (!typeof(ITenantEntity).IsAssignableFrom(entityType)) return;

            // Tenant
            builder
                .Property<Guid?>(ShadowProperties.TenantId);
        }
    }
}
