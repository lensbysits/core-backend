using System;
using System.ComponentModel.DataAnnotations;

namespace Lens.Core.Blob.Models
{
    public class BlobInfoMergeModel
    {
        [StringLength(20)]
        public string ContentType { get; set; }
        [Required]
        public Guid? EntityId { get; set; }
        [StringLength(532)]
        public string FilenameWithExtension { get; set; }
        public int Size { get; set; }
        public string Tag { get; set; }
    }
}
