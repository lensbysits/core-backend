using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lens.Core.Blob.Models;

public class BlobInfoInputModel
{
    [Required]
    public Guid? EntityId { get; set; }
    [StringLength(532)]
    public bool SkipFileDeletion { get; set; } = false;
    [JsonIgnore]
    public string Tag => JsonSerializer.Serialize(Tags ?? Array.Empty<string>());
    public string[]? Tags { get; set; }
}
