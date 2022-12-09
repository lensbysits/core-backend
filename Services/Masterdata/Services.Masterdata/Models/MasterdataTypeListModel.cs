namespace Lens.Services.Masterdata.Models;

public class MasterdataTypeListModel
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? MasterdatasCount { get; set; }
}
