using Lens.Core;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lens.Services.Masterdata.Models;

public class MasterdataImportModel
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    [StringLength(50), Required]
    public string? Code { get; set; }
    public IEnumerable<MasterdataCreateModel> Masterdatas { get; set; } = Enumerable.Empty<MasterdataCreateModel>();

    [JsonIgnore]
    public string MetadataJson
    {
        get
        {
            return Metadata.HasValue ? JsonSerializer.Serialize(Metadata) : JsonNodeUtilities.EmptyObjectJson;
        }
    }

    public JsonElement? Metadata { get; set; }
}
