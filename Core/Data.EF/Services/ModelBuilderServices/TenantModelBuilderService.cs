using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace CoreApp.Data.Services
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
