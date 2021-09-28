using CoreLib.Builders;
using Lamar;
using Lamar.Scanning.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoreApp.Web
{
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
            var applicationSetup = new ApplicationSetupBuilder(services, Configuration);
            OnSetupApplication(applicationSetup);

            services
                .AddAuthentication(Configuration)
                .AddCors(Configuration);

            services.AddSwaggerGen();

            services.AddControllers(config =>
            {
                config.Filters.Add(new AuthorizeFilter());
            });

            applicationSetup.AddApplicationServices();
        }

        public virtual void OnSetupApplication(IApplicationSetupBuilder applicationSetup) { }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();

            app
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("swagger/v1/swagger.json", "API V1");
                    c.RoutePrefix = string.Empty;
                });

            app.UseAuthentication();

            app.UseErrorHandling();

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
}
