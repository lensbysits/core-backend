using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lens.Services.Masterdata.Models;

public class MasterdataCreateModel
{
    public Guid MasterdataTypeId { get; set; }
    [StringLength(50), Required]
    public string? Key { get; set; }
    [StringLength(50), Required]
    public string? Value { get; set; }
    [StringLength(50)]
    public string? Name { get; set; }
    [StringLength(1024)]
    public string? Description { get; set; }

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
