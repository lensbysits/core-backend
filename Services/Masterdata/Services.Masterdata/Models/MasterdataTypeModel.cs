using Lens.Core.Lib.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lens.Services.Masterdata.Models;

public class MasterdataTypeModel : IdModel, IMetadataModel
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? MasterdatasCount { get; set; }

    [JsonIgnore]
    public string? MetadataJson { get; set; }

    public dynamic? Metadata 
    { 
        get
        {
            if((Domain ?? IMetadataModel.AllDomains) == IMetadataModel.AllDomains)
            {
                return MetadataDictionary;
            }
            else
            {
                return MetadataDictionary!.TryGetValue(Domain!, out var value) ? value : null;
            }
        }
    }

    public string? Domain { get; set; } = IMetadataModel.AllDomains;

    private Dictionary<string, dynamic>? metadataDictionary;
    [JsonIgnore]
    public Dictionary<string, dynamic>? MetadataDictionary => metadataDictionary ??= JsonSerializer.Deserialize<Dictionary<string, dynamic>>(MetadataJson ?? "{}");
}
