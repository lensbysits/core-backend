namespace Lens.Core.App.Web.Options
{
    internal class ControllerOptions : IControllerOptions
    {
        public bool IgnoreResultModelWrapper { get; private set; } = false;



        public IControllerOptions IgnoreResultModelWrapping()
        {
            IgnoreResultModelWrapper = true;
            return this;
        }
    }
}
