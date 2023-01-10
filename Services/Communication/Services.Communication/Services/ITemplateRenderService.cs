using Lens.Services.Communication.Models;

namespace Lens.Services.Communication;

public interface ITemplateRenderService
{
    Task<string> RenderToStringAsync(string viewName, object? model);
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class TemplateRendererServiceTypeAttribute : Attribute
{
    public TemplateRendererServiceTypeAttribute(TemplateTypeEnum type)
    {
        Type = type;
    }

    public TemplateTypeEnum Type { get; private set; }
}
