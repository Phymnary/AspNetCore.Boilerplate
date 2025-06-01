using AspNetCore.Boilerplate.Domain;
using FluentValidation;

namespace AspNetCore.Boilerplate.EntityFrameworkCore;

public abstract class EfRepositoryOptions<TEntity> : IRepositoryOptions<TEntity>
    where TEntity : class, IEntity
{
    public IValidator<TEntity>? Validator { get; protected init; }

    public EntityQueryOptions<TEntity>? QueryOptions { get; protected init; }

    public EntityUpdateOptions<TEntity>? UpdateOptions { get; protected init; }
}
