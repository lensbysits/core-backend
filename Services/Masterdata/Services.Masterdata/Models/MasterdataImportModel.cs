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
    public IEnumerable<MasterdataCreateBM> Masterdatas { get; set; } = Enumerable.Empty<MasterdataCreateBM>();

    [JsonIgnore]
    public string MetadataJson
    {
        get
        {
            return JsonSerializer.Serialize(Metadata ?? new { });
        }
    }

    public dynamic? Metadata { get; set; }
}
