using Lens.Core.Data.EF.AuditTrail.Entities;
using Lens.Core.Data.EF.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lens.Core.Blob.Data.Entities;

[Table("BlobInfo", Schema = "blob")]
public class BlobInfo : BaseEntity, ITagsEntity, IAuditTrailEntity
{
    [StringLength(128)]
    public string? ContentType { get; set; }
    public Guid? EntityId { get; set; }
    [StringLength(20)]
    public string? FileExtension { get; set; }
    [StringLength(532)]
    public string? FilenameWithExtension { get; set; }
    [StringLength(512)]
    public string? FilenameWithoutExtension { get; set; }
    [StringLength(2048)]
    public string? FullPathAndName { get; set; }
    [StringLength(2048)]
    public string RelativePathAndName { get; set; } = null!;
    public int Size { get; set; }
    public bool SkipFileDeletion { get; set; }
    [StringLength(2048)]
    public string? Tag { get; set; }
}
