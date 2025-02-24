using System.Linq.Expressions;

namespace AspNetCore.Boilerplate.Domain;

public interface IRepository<TEntity, in TId> : IAsyncDisposable, IDisposable
    where TEntity : Entity<TId>
{
    IQueryable<TEntity> Queryable(bool isIncludeDetails = false);

    Task<TEntity> InsertAsync(
        TEntity entity,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Return Entity based on primary key
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <param name="isIncludeDetails">get related entities</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <exception cref="EntityNotFoundException">If repository can not find entity</exception>
    Task<TEntity> GetAsync(
        TId id,
        bool isIncludeDetails = false,
        CancellationToken cancellationToken = default
    );

    Task<TEntity?> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool isIncludeDetails = false,
        CancellationToken cancellationToken = default
    );

    Task<List<TEntity>> QueryAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        bool isIncludeDetails = false,
        CancellationToken cancellationToken = default
    );

    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    );

    Task<int> CountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    );

    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
}
