using Lens.Core.Data.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EFCore = Microsoft.EntityFrameworkCore.EF;

namespace Lens.Core.Data.EF.Services;

public class RecordStateModelBuilderService : IModelBuilderService
{
    public void ConfigureBaseProperties(Type entityType, EntityTypeBuilder builder)
    {
        // RecordState
        if (!typeof(IRecordStateEntity).IsAssignableFrom(entityType)) return;
        
        builder
            .Property<RecordStateEnum>(ShadowProperties.RecordState)
            .HasDefaultValue(RecordStateEnum.NotDefined);

        builder
            .AppendQueryFilter<IRecordStateEntity>(e => EFCore.Property<RecordStateEnum>(e, ShadowProperties.RecordState) != RecordStateEnum.Deleted);
    }
}
