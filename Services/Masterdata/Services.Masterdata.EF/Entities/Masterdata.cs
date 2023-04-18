using Lens.Core.Data.EF.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lens.Services.Masterdata.EF.Entities;

public class Masterdata : BaseEntity, ITagsEntity, ITranslationEntity
{
    public Guid MasterdataTypeId { get; set; }
    public virtual MasterdataType MasterdataType { get; set; } = default!;

    [StringLength(50), Required]
    public string? Key { get; set; }
    [StringLength(50), Required]
    public string? Value { get; set; }
    [StringLength(50)]

    public string? Name { get; set; }
    [StringLength(1024)]
    public string? Description { get; set; }
    public string? MetadataJson { get; set; }

    [StringLength(2048)]
    public string? Tag { get; set; }

    public string? Translation { get; set; }

    public virtual ICollection<MasterdataKey> MasterdataKeys { get; set; } = new HashSet<MasterdataKey>();

    public virtual ICollection<MasterdataRelated> ParentMasterdata { get; set; } = new HashSet<MasterdataRelated>();

    public virtual ICollection<MasterdataRelated> ChildMasterdata { get; set; } = new HashSet<MasterdataRelated>();
}
