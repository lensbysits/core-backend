using Lamar;
using Lamar.Scanning.Conventions;
using Lens.Core.App.Web.Authentication;
using Lens.Core.App.Web.Builders;
using Lens.Core.App.Web.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;

namespace Lens.Core.App.Web;

public class StartupBase
{
    protected readonly IConfiguration Configuration;

    public StartupBase(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureContainer(ServiceRegistry services)
    {
        services.Scan(scanner =>
        {
            scanner.TheCallingAssembly();
            scanner.WithDefaultConventions();

            OnAssemblyScanning(scanner);
        });

        OnConfigureContainer(services);
    }

    public virtual void OnAssemblyScanning(IAssemblyScanner scanner) { }
    public virtual void OnConfigureContainer(ServiceRegistry services) { }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        var applicationSetup = new WebApplicationSetupBuilder(services, Configuration);
        OnSetupApplication(applicationSetup);

        services
            .AddAuthentication(Configuration)
            .AddCors(Configuration)
            .AddSwagger(Configuration);

        var mvcBuilder = applicationSetup.ControllerOptions.UsingViews ?
            services.AddControllersWithViews(options => ConfigureControllers(options, applicationSetup)) :
            services.AddControllers(options => ConfigureControllers(options, applicationSetup));

        mvcBuilder.AddJsonOptions(options => ConfigureJsonOptions(options, applicationSetup));

        applicationSetup.AddApplicationServices();
    }

    private static void ConfigureControllers(MvcOptions options, WebApplicationSetupBuilder applicationSetup)
    {
        var authMethod = AuthenticationFactory.GetAuthenticationMethod(applicationSetup.Configuration);
        options.Filters.Add(new AuthorizeFilter());

        if (!applicationSetup.ControllerOptions.IgnoreResultModelWrapper)
        {
            options.Filters.Add(new ResultModelWrapperFilter());
        }

        authMethod.ApplyMvcFilters(options.Filters);
    }

    private static void ConfigureJsonOptions(JsonOptions options, WebApplicationSetupBuilder applicationSetup)
    {
        if (applicationSetup.ControllerOptions.JsonEnumsAsStrings)
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        if (applicationSetup.ControllerOptions.JsonIgnoreNullProperties)
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        }
    }

    public virtual void OnSetupApplication(IWebApplicationSetupBuilder applicationSetup) { }

    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseCors();

        app
            .UseSwagger(Configuration)
            .UseSwaggerUI(Configuration);

        app.UseAuthentication(Configuration);

        app.UseErrorHandling();

        app.UseHealthChecks("/health");

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            UseEndpoints(endpoints);
        });
    }

    protected virtual void UseEndpoints(IEndpointRouteBuilder endpoints) { }
}
