using Lens.Core.Lib.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lens.Core.Blob.Models;

public class BlobInfoModel : IdModel
{
    public string? ContentType { get; set; }
    public Guid EntityId { get; set; }
    public string? FileExtension { get; set; }
    public string? FilenameWithExtension { get; set; }
    public string? FilenameWithoutExtension { get; set; }
    public string? FullPathAndName { get; set; }
    public string? RelativePathAndName { get; set; }
    public int Size { get; set; }
    public bool SkipFileDeletion { get; set; }
    [JsonIgnore]
    public string Tag
    {
        set => Tags = JsonSerializer.Deserialize<string[]>(value ?? "[]");
    }
    public string[]? Tags { get; set; }
}
