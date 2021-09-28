using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace CoreApp.Data.Services
{
    public class CreateUpdateModelBuilderService : IModelBuilderService
    {
        public void ConfigureBaseProperties(Type entityType, EntityTypeBuilder builder)
        {
            
            if (!typeof(ICreatedUpdatedEntity).IsAssignableFrom(entityType)) return;

            // Created/Updated On/By
            builder
                .Property<DateTime>(ShadowProperties.CreatedOn)
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();

            builder
                .Property<DateTime>(ShadowProperties.UpdatedOn)
                .HasDefaultValueSql("GETDATE()")
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
