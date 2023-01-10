using Fluid;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Lens.Services.Communication;

[TemplateRendererServiceType(Models.TemplateTypeEnum.Liquid)]
public class LiquidTemplateRenderService : ITemplateRenderService
{
    private readonly ILogger<LiquidTemplateRenderService> _logger;
    private readonly FluidParser _fluidParser;

    public LiquidTemplateRenderService(
        ILogger<LiquidTemplateRenderService> logger,
        FluidParser fluidParser)
    {
        _logger = logger;
        _fluidParser = fluidParser;
    }

    public async Task<string> RenderToStringAsync(string view, object? model)
    {
        try
        {
            if (_fluidParser.TryParse(view, out var template, out var errors))
            {
                var options = new TemplateOptions();
                options.MemberAccessStrategy.Register<JsonElement, object>((obj, name) => {
                    var element = obj.GetProperty(name);
                    return element.ValueKind != JsonValueKind.Object ? element.GetString() ?? string.Empty : element;
                });
                var context = new TemplateContext(model, options);
                var body = await template.RenderAsync(context);
                return body;
            }
            else
            {
                throw new Exception();
            }
        }

        catch (Exception e)
        {
            _logger.LogError(e, "Error while parsing liquid email-template");
            throw;
        }
    }
}