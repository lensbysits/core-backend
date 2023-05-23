namespace Lens.Core.Data.EF.Entities;

public interface IIdEntity : IEntity
{
    Guid Id { get; set; }
}
