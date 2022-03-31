using System.ComponentModel.DataAnnotations;

namespace Lens.Core.Lib.Models
{
    public class BlobInfoMergeModel
    {
        [StringLength(20)]
        public string ContentType { get; set; }
        [StringLength(20)]
        public string FileExtension { get; set; }
        [StringLength(532)]
        public string FilenameWithExtension { get; set; }
        [StringLength(512)]
        public string FilenameWithoutExtension { get; set; }
        [StringLength(2048)]
        public string FullPathAndName { get; set; }
        [MaxLength(1024 * 1024)]
        public byte[] Image { get; set; }
		[StringLength(20)]
		public string ImageType { get; set; }
		[StringLength(2048)]
		public string RelativePathAndName { get; set; }
		public int Size { get; set; }
	}
}
