using Lens.Services.Communication.Models;

namespace Lens.Services.Communication;

public class TemplateRenderServiceFactory : ITemplateRenderServiceFactory
{
    private readonly IEnumerable<ITemplateRenderService> _templateRenderServices;

    public TemplateRenderServiceFactory(IEnumerable<ITemplateRenderService> templateRenderServices)
    {
        _templateRenderServices = templateRenderServices;
    }

    public ITemplateRenderService GetTemplateRenderService(TemplateTypeEnum type)
    {
        return _templateRenderServices.
            FirstOrDefault(r => r.GetType().GetCustomAttributes(
                                                typeof(TemplateRendererServiceTypeAttribute), false)
                                                .Cast<TemplateRendererServiceTypeAttribute>()
                                                .FirstOrDefault()?.Type == type) ??
                                         throw new Exception($"TemplateRenderService for type {type} was not registered.");
    }
}
