namespace Lens.Services.Communication;

[TemplateRendererServiceType(Models.TemplateTypeEnum.Plain)]
public class PlainTemplateRenderService : ITemplateRenderService
{

    public Task<string> RenderToStringAsync(string template, object? model)
    {
        return Task.FromResult(template);
    }
}