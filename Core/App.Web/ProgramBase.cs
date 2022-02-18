using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Lens.Core.App.Web
{
    public abstract class ProgramBase<TStartup> : ProgramBase where TStartup: class
    {
        public static async Task<int> Start(string[] args) =>
            await Start(args, CreateWebHostBuilder);

        protected static IHostBuilder CreateWebHostBuilder(string[] args) =>
            CreateBaseHostBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseSetting(WebHostDefaults.CaptureStartupErrorsKey, "true")
                        .UseStartup<TStartup>();
                });
    }
}
