using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lens.Core.Lib.Models;

public class MetadataModel : IMetadataModel
{
    public string? Domain { get; set; } = IMetadataModel.AllDomains;
    public JsonElement? Metadata { get; set; }

    private Dictionary<string, JsonElement>? metadataDictionary;
    [JsonIgnore]
    public Dictionary<string, JsonElement>? MetadataDictionary => metadataDictionary ??= JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(Metadata?.ToString() ?? JsonNodeUtilities.EmptyObjectJson);
}
