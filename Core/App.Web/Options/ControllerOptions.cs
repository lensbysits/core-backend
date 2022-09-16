namespace Lens.Core.App.Web.Options;

internal class ControllerOptions : IControllerOptions
{
    public bool IgnoreResultModelWrapper { get; private set; } = false;
    public bool JsonEnumsAsStrings { get; private set; } = false;
    public bool JsonIgnoreNullProperties { get; private set; } = false;
    public bool UsingViews { get; private set; } = false;

    public IControllerOptions IgnoreResultModelWrapping()
    {
        IgnoreResultModelWrapper = true;
        return this;
    }

    public IControllerOptions JsonSerializeEnumsAsStrings()
    {
        JsonEnumsAsStrings = true;
        return this;
    }

    public IControllerOptions JsonSerializeIgnoreNullProperties()
    {
        JsonIgnoreNullProperties = true;
        return this;
    }

    public IControllerOptions UseViews()
    {
        UsingViews = true;
        return this;
    }
}
