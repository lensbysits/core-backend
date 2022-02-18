namespace Lens.Core.Data.EF.Entities
{
    public interface IImageEntity
    {
        public byte[] Image { get; set; }
        public string ImageType { get; set; }
    }
}
