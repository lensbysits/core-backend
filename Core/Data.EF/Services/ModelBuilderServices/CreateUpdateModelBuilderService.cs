using Lens.Core.Data.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Lens.Core.Data.EF.Services
{
    public class CreateUpdateModelBuilderService : IModelBuilderService
    {
        public void ConfigureBaseProperties(Type entityType, EntityTypeBuilder builder)
        {
            
            if (!typeof(ICreatedUpdatedEntity).IsAssignableFrom(entityType)) return;

            // Created/Updated On/By
            builder
                .Property<DateTime>(ShadowProperties.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();

            builder
                .Property<DateTime>(ShadowProperties.UpdatedOn)
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();

            builder
                .Property<string>(ShadowProperties.CreatedBy)
                .HasMaxLength(50);

            builder
                .Property<string>(ShadowProperties.UpdatedBy)
                .HasMaxLength(50);
        }
    }
}
