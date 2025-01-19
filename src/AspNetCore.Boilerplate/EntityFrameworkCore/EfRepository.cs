using System.Linq.Expressions;
using AspNetCore.Boilerplate.Domain;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Boilerplate.EntityFrameworkCore;

public class EfRepository<TDbContext, TEntity, TId>(
    TDbContext context,
    ICurrentUser currentUser,
    EfEntityQueryOptions<TEntity>? queryOptions = null
) : IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TDbContext : DbContext
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    private readonly EfEntityQueryOptions<TEntity> _queryOptions =
        queryOptions ?? new EfEntityQueryOptions<TEntity>();

    public async Task<TEntity> InsertAsync(
        TEntity entity,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var inserted = _dbSet.Add(entity);
        if (autoSave)
            await context.SaveChangesAsync(cancellationToken);

        return inserted.Entity;
    }

    public async Task<TEntity> GetAsync(
        TId id,
        bool isIncludeDetails = false,
        CancellationToken cancellationToken = default
    )
    {
        return await Queryable(isIncludeDetails)
                .FirstOrDefaultAsync(item => item.Id!.Equals(id), cancellationToken)
            ?? throw new EntityNotFoundException(
                $"Could not find entity {typeof(TEntity).Name} with id {id}"
            );
    }

    public async Task<TEntity?> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool isIncludeDetails = false,
        CancellationToken cancellationToken = default
    )
    {
        return await Queryable(isIncludeDetails).FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<List<TEntity>> QueryAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool isIncludeDetails = false,
        CancellationToken cancellationToken = default
    )
    {
        return await Queryable(isIncludeDetails).Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public async Task<TEntity> UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default
    )
    {
        var now = DateTime.Now;
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is not IAuditable auditable)
                continue;

            switch (entry.State)
            {
                case EntityState.Modified:
                    auditable.UpdatedAt = now;
                    auditable.UpdatedById = currentUser.Id;
                    break;

                case EntityState.Added:
                    auditable.CreatedAt = now;
                    auditable.CreatedById = currentUser.Id;
                    break;

                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    break;
                case EntityState.Deleted:
                    break;
                default:
                    continue;
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public void Dispose()
    {
        if (context is IDisposable contextDisposable)
            contextDisposable.Dispose();
        else
            _ = context.DisposeAsync().AsTask();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await context.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public IQueryable<TEntity> Queryable(bool isIncludeDetails = false)
    {
        var queryable = _queryOptions.DefaultIncludeQuery.Invoke(_dbSet);
        if (isIncludeDetails)
            queryable = _queryOptions.IncludeDetailsQuery.Invoke(queryable);

        return queryable;
    }
}
