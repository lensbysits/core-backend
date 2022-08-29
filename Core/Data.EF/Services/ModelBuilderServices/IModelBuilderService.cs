using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lens.Core.Data.EF.Services;

public interface IModelBuilderService
{
    void ConfigureBaseProperties(Type entityType, EntityTypeBuilder builder);
}
