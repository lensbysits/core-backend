using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace CoreApp.Data.Services
{
    public class TagsModelBuilderService : IModelBuilderService
    {
        public void ConfigureBaseProperties(Type entityType, EntityTypeBuilder builder)
        {
            if (!typeof(ITagsEntity).IsAssignableFrom(entityType)) return;

            // Tags
            builder
                .Property<string>(ShadowProperties.Tag);
        }
    }
}
