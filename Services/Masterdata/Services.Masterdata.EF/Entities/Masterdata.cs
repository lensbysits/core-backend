using Lens.Core.Data.EF.Entities;
using System.ComponentModel.DataAnnotations;

namespace Lens.Services.Masterdata.EF.Entities;

public class Masterdata : BaseEntity
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
}
