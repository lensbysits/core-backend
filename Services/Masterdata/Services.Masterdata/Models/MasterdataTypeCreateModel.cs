using Lens.Core.Lib.Models;
using System.ComponentModel.DataAnnotations;

namespace Lens.Services.Masterdata.Models;

public  class MasterdataTypeCreateModel : MetadataModel
{
    [StringLength(50), Required]
    public string? Code { get; set; }
    [StringLength(50)]
    public string? Name { get; set; }
    [StringLength(1024)]
    public string? Description { get; set; }
}
