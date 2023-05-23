using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lens.Core.Lib.Models;

public interface IMetadataModel
{
    public const string AllDomains = "*";

    JsonElement? Metadata { get; }
    [JsonIgnore]
    Dictionary<string, JsonElement>? MetadataDictionary { get; }
    string? Domain { get; set; }
}
