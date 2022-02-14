using Lens.Core.Data.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Lens.Core.Data.EF.Entities
{
    public abstract class BaseEntity : IBaseEntity
    {
        public Guid Id { get; set; }
    }

    public abstract class BaseEntityWithImage : BaseEntity, IImageEntity
    {
        [NonAudit] 
        [MaxLength(1024 * 1024)]
        public byte[] Image { get; set; }

        [StringLength(20)]
        public string ImageType { get; set; }
    }
}
