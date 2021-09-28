using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace CoreApp.Data.Services
{
    public class RecordStateModelBuilderService : IModelBuilderService
    {
        public void ConfigureBaseProperties(Type entityType, EntityTypeBuilder builder)
        {
            // RecordState
            if (!typeof(IRecordState).IsAssignableFrom(entityType)) return;
            
            builder
                .Property<RecordStateEnum>(ShadowProperties.RecordState)
                .HasDefaultValue(RecordStateEnum.NotDefined);

            builder
                .AppendQueryFilter<IRecordState>(e => EF.Property<RecordStateEnum>(e, ShadowProperties.RecordState) != RecordStateEnum.Deleted);
        }
    }
}
