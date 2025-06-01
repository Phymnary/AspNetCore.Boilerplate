using System.Linq.Expressions;
using AspNetCore.Boilerplate.EntityFrameworkCore;
using AspNetCore.Boilerplate.EntityFrameworkCore.Pagination;

namespace AspNetCore.Boilerplate.Domain;

public interface IRepository<TEntity>
    where TEntity : class, IEntity
{
    internal IQueryable<TEntity> Queryable(bool isIncludeDetails = false);

    /// <summary>
    /// Insert entity to database context
    /// </summary>
    /// <param name="entity">Entity to be inserted.</param>
    /// <param name="autoSave">Call SaveChanges or not.</param>
    /// <param name="stoppingToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">Database exceptions.</exception>
    /// <exception cref="EntityValidationException">Throw by validator.</exception>
    Task<TEntity> InsertAsync(
        TEntity entity,
        bool autoSave = true,
        CancellationToken stoppingToken = default
    );

    /// <summary>
    /// Insert entity to database context
    /// </summary>
    /// <param name="entity">Entity to be inserted or updated if existed.</param>
    /// <param name="on">Expression to check if entity is existed or not</param>
    /// <param name="autoSave">Call SaveChanges or not.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">Database exceptions.</exception>
    /// <exception cref="EntityValidationException">Throw by validator.</exception>
    Task<TEntity> UpsertAsync(
        TEntity entity,
        Expression<Func<TEntity, bool>> on,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    );

    ///<summary>
    /// Validate entity then run all <see cref="AspNetCore.Boilerplate.EntityFrameworkCore.Interceptors.IEfSaveChangesInterceptor"/> and SaveChanges
    /// </summary>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">Database exceptions.</exception>
    /// <exception cref="EntityValidationException">Throw by validator.</exception>
    Task<int> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

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

    IPaginateOrderBuilding<TEntity> Paginate(
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? filter = null
    );
}
