using Lens.Core.Data.EF.Services;
using Lens.Core.Lib.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace Lens.Core.Data.EF.Extensions;

public static class DatabaseSetupBuilderExtensions
{
    public static IApplicationSetupBuilder AddModelBuilderService(this IApplicationSetupBuilder builder)
    {
        // add model builder services
        builder.Services.AddTransient<IModelBuilderService, CreateUpdateModelBuilderService>();
        builder.Services.AddTransient<IModelBuilderService, RecordStateModelBuilderService>();
        builder.Services.AddTransient<IModelBuilderService, TagsModelBuilderService>();
        builder.Services.AddTransient<IModelBuilderService, TranslationModelBuilderService>();
        builder.Services.AddTransient<IModelBuilderService, TenantModelBuilderService>();

        return builder;
    }
}
