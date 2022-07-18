using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lens.Core.Blob.Models
{
    public class BlobInfoBulkCreateModel
    {
        public IEnumerable<BlobInfoInputModel> BlobInfoItems { get; set; }
        [StringLength(20)]
        public string ContentType { get; set; }
        [StringLength(532)]
        public string FilenameWithExtension { get; set; }
        public int Size { get; set; }
    }
}
