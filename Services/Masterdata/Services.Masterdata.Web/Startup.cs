using Lens.Core.App.Web.Builders;
using Lens.Services.Masterdata.EF;

namespace Lens.Services.Masterdata.Web;

public class Startup : Core.App.Web.StartupBase
{
    public Startup(IConfiguration configuration) : base(configuration)
    {
    }

    public override void OnSetupApplication(IWebApplicationSetupBuilder applicationSetup)
    {
        applicationSetup
            // Add app specific services.
            .AddMasterdata();
    }
}
