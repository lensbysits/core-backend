using Lens.Core.Data.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lens.Core.Data.EF.Services;

public class IdModelBuilderService : IModelBuilderService
{
    public void ConfigureBaseProperties(Type entityType, EntityTypeBuilder builder)
    {

        if (!typeof(IIdEntity).IsAssignableFrom(entityType)) return;

        // IId entity
        if (builder.Property(nameof(IIdEntity.Id)).Metadata.ClrType == typeof(Guid))
        {
            builder
                .Property(nameof(IIdEntity.Id))
                .HasDefaultValueSql("newsequentialid()")
                .ValueGeneratedOnAdd();
        }

        builder
            .Property<byte[]>(ShadowProperties.Timestamp)
            .IsConcurrencyToken()
            .ValueGeneratedOnAddOrUpdate();
    }
}
