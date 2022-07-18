using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lens.Core.Blob.Models
{
    public class BlobInfoCreateModel
    {
        [StringLength(20)]
        public string ContentType { get; set; }
        [Required]
        public Guid? EntityId { get; set; }
        [StringLength(532)]
        public string FilenameWithExtension { get; set; }
        public int Size { get; set; }
        public bool SkipFileDeletion { get; set; }
        [JsonIgnore]
        public string Tag => JsonSerializer.Serialize(Tags ?? Array.Empty<string>());
        public string[] Tags { get; set; }
    }
}
