using System.Text.Json.Serialization;

namespace Lens.Core.Lib.Models;

public class MetadataModel : IMetadataModel
{
    public string? Domain { get; set; } = IMetadataModel.AllDomains;
    public dynamic? Metadata { get; set; }
    [JsonIgnore]
    public Dictionary<string, dynamic>? MetadataDictionary { get; set; }
}
