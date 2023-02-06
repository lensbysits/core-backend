namespace Lens.Core.Data.EF.Entities;

public interface ITagsEntity
{
    /// <summary>
    /// This property contains multiple tags, separated by comma. Ex.: green, red, blue
    /// </summary>
    string? Tag { get; set; }
}
