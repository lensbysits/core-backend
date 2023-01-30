using Lens.Core.Lib.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

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
                Metadata = JsonSerializer.Deserialize<JsonElement>(value);
            }
        }
    }

    public JsonElement? Metadata { get; set; }
}
