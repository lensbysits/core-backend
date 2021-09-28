using CoreLib.Builders;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace CoreApp.Console
{
    public abstract class ProgramBase : Program.ProgramBase
    {
        private static Action<HostBuilderContext, IApplicationSetupBuilder> _applicationSetup;
        protected static Action<HostBuilderContext, IApplicationSetupBuilder> ApplicationSetup
        {
            get => _applicationSetup;
            set => _applicationSetup = value;
        }

        public static async Task<int> Start(string[] args) =>
            await Start(args, CreateHostBuilder);

        protected static IHostBuilder CreateHostBuilder(string[] args) =>
                CreateBaseHostBuilder(args)
                    .UseConsoleLifetime()
                    .ConfigureServices((context, services) =>
                    {
                        var applicationSetup = new ApplicationSetupBuilder (services, context.Configuration);
                        ApplicationSetup?.Invoke(context, applicationSetup);
                        applicationSetup.AddApplicationServices();
                    });
    }
}
