using Lens.Core.Data.EF.Entities;
using System.ComponentModel.DataAnnotations;

namespace Lens.Services.Masterdata.EF.Entities
{
    public class MasterdataType : IIdEntity, ICreatedUpdatedEntity
    {
        public Guid Id { get; set; }
        [StringLength(50), Required]
        public string? Code { get; set; }
        [StringLength(50)]
        public string? Name { get; set; }
        [StringLength(1024)]
        public string? Description { get; set; }
        public string? MetadataJson { get; set; }
        public virtual ICollection<Masterdata> Masterdatas { get; set; } = new HashSet<Masterdata>();
    }
}
