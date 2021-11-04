using System;
using System.ComponentModel.DataAnnotations;

namespace Lens.Core.Data.EF.Entities
{
    public abstract class EntityBase : IEntityBase
    {
        public Guid Id { get; set; }
    }

    public abstract class EntityBaseWithImage : EntityBase, IImageEntity
    {
        //TODO: Add non-audit functionallity
        //[NonAudit] 
        [MaxLength(1024 * 1024)]
        public byte[] Image { get; set; }

        [StringLength(20)]
        public string ImageType { get; set; }
    }
}
