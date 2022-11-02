using Lens.Services.Communication.Models;

namespace Lens.Services.Communication;

public interface ITemplateRenderServiceFactory
{
    ITemplateRenderService GetTemplateRenderService(TemplateTypeEnum type);
}