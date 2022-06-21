using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lens.Core.Blob.Models
{
    public class BlobInfoBulkCreateModel
    {
        [StringLength(20)]
        public string ContentType { get; set; }
        public IEnumerable<Guid> EntityIds { get; set; }
        [StringLength(532)]
        public string FilenameWithExtension { get; set; }
        public int Size { get; set; }
        [JsonIgnore]
        public string Tag => JsonSerializer.Serialize(Tags ?? Array.Empty<string>());
        public string[] Tags { get; set; }
    }
}
