using System.Text.Json;
using System.Text.Json.Serialization;
using Lens.Core.Lib.Models;

namespace Lens.Services.Masterdata.Models;

public class MasterdataModel : IdModel
{
    public Guid MasterdataTypeId { get; set; }
    public string? MasterdataTypeName { get; set; }
    public string? Key { get; set; }
    public string? Value { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    [JsonIgnore]
    public string MetadataJson
    {
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                Metadata = JsonSerializer.Deserialize<dynamic>(value);
            }
        }
    }

    public dynamic? Metadata { get; set; }
}
