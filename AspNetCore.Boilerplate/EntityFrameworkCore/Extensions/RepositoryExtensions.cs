using AspNetCore.Boilerplate.Domain;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Boilerplate.EntityFrameworkCore.Extensions;

public static class RepositoryExtensions
{
    public static async Task<TEntity> GetAsync<TEntity, TId>(
        this IRepository<TEntity> repository,
        TId id,
        bool isIncludeDetails = false,
        CancellationToken cancellationToken = default
    )
        where TEntity : Entity<TId>
        where TId : IEquatable<TId>
    {
        return await repository
                .Queryable(isIncludeDetails)
                .FirstOrDefaultAsync(entity => entity.Id.Equals(id), cancellationToken)
            ?? throw new EntityNotFoundException(
                $"Could not find entity {typeof(TEntity).Name} with id {id}"
            );
    }
}
