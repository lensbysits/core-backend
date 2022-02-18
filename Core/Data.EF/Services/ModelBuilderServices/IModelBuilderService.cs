using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Lens.Core.Data.EF.Services
{
    public interface IModelBuilderService
    {
        void ConfigureBaseProperties(Type entityType, EntityTypeBuilder builder);
    }
}
