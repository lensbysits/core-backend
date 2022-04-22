using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lens.Services.Masterdata.Models;

public  class MasterdataTypeCreateModel
{
    [StringLength(50), Required]
    public string? Code { get; set; }
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
