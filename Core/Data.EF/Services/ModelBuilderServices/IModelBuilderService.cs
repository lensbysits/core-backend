using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace CoreApp.Data.Services
{
    public interface IModelBuilderService
    {
        void ConfigureBaseProperties(Type entityType, EntityTypeBuilder builder);
    }
}
