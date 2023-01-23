using Lens.Core.Lib.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lens.Services.Masterdata.Models;

public class MasterdataTagModel : IdModel
{
    [JsonIgnore]
    public string Tag
    {
        set => Tags = JsonSerializer.Deserialize<string[]>(value ?? "[]");
    }
    public string[]? Tags { get; set; }
}
