using System.Text.Json.Serialization;

namespace Lens.Core.Lib.Models;

public interface IMetadataModel
{
    public const string AllDomains = "*";

    dynamic? Metadata { get; }
    [JsonIgnore]
    Dictionary<string, dynamic>? MetadataDictionary { get; }
    string? Domain { get; set; }
}
