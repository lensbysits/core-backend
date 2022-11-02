namespace Lens.Services.Communication.Models;

public class EmailTemplateBM
{
    public string? Template { get; set; }
    public TemplateTypeEnum TemplateType { get; set; } = TemplateTypeEnum.Plain;
}
