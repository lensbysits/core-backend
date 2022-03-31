namespace Lens.Core.Lib.Models
{
    public class BlobInfoModel : IdModel
    {
        public string ContentType { get; set; }
        public string FileExtension { get; set; }
        public string FilenameWithExtension { get; set; }
        public string FilenameWithoutExtension { get; set; }
        public string FullPathAndName { get; set; }
        public string RelativePathAndName { get; set; }
        public int Size { get; set; }
	}
}
