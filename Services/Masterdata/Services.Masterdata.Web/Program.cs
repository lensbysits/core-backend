using Lens.Core.App.Web;

namespace Lens.Services.Masterdata.Web;

public class Program : ProgramBase<Startup>
{
    public static async Task<int> Main(string[] args) => await Start(args);

    public static IHostBuilder CreateHostBuilder(string[] args) => CreateWebHostBuilder(args);
}
