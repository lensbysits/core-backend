using Microsoft.AspNetCore.Mvc.Filters;

namespace Lens.Core.App.Web.Options;

internal class ControllerOptions : IControllerOptions
{
    private ICollection<Type> requestPipelineFilterMetadata { get; }
    public bool IgnoreResultModelWrapper { get; private set; } = false;
    public bool JsonEnumsAsStrings { get; private set; } = false;
    public bool JsonIgnoreNullProperties { get; private set; } = false;
    public bool UsingViews { get; private set; } = false;

    public ControllerOptions()
    {
        this.requestPipelineFilterMetadata = new List<Type>();
    }

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

    public IControllerOptions AddRequestPipeLineFilter<T>() where T : IFilterMetadata
    {
        this.requestPipelineFilterMetadata.Add(typeof(T));
        return this;
    }

    public ICollection<Type> GetRequestPipeLineFilters()
    {
        return this.requestPipelineFilterMetadata;
    }
}
