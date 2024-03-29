﻿using Lens.Core.Data.EF.AuditTrail.Services;
using Lens.Core.Data.Services;
using Lens.Core.Lib;
using Lens.Core.Lib.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lens.Core.Data.EF.AuditTrail;

public static class AuditTrailSetupBuilderExtensions
{
    public static IApplicationSetupBuilder AddAuditTrailing(this IApplicationSetupBuilder builder, 
        string connectionStringName = "DefaultConnection", 
        string connectionStringPassword = "dbPassword",
        Action<IServiceProvider, DbContextOptionsBuilder>? dbContextOptions = null)
    {
        builder
            .AddProgramInitializer<AuditTrailInitializerService>()
            .AddAssemblies(typeof(AutoMapperProfile).Assembly)
            .AddSqlServerDatabase<AuditTrailDbContext>(
                connectionStringName, 
                connectionStringPassword, 
                typeof(AuditTrailSetupBuilderExtensions).Assembly,
                dbContextOptions)
            .Services
                .AddScoped<IAuditTrailService, AuditTrailService>();

        return builder;
    }
}
