using Lens.Core.Data.EF.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lens.Core.Data.EF.Services;

public class TagsModelBuilderService : IModelBuilderService
{
    public void ConfigureBaseProperties(Type entityType, EntityTypeBuilder builder)
    {
        if (!typeof(ITagsEntity).IsAssignableFrom(entityType)) return;

        // Tags
        builder
            .Property<string>(ShadowProperties.Tag)
            .HasMaxLength(2048); 
    }
}
