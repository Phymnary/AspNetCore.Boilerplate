using System.ComponentModel.DataAnnotations;

namespace AspNetCore.Boilerplate.Domain;

public interface IEntity;

public abstract class Entity<TKey> : IEntity
{
    protected Entity() { }

    protected Entity(TKey id)
    {
        Id = id;
    }

    [Key]
    public TKey Id { get; protected init; } = default!;
}
