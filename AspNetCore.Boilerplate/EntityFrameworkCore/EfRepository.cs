using System.Linq.Expressions;
using AspNetCore.Boilerplate.Application.Extensions;
using AspNetCore.Boilerplate.Domain;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Boilerplate.EntityFrameworkCore;

public abstract class EfRepository<TDbContext, TEntity>(
    TDbContext context,
    IRepositoryOptions<TEntity>? options = null
) : IRepository<TEntity>
    where TEntity : class, IEntity
    where TDbContext : DbContext
{
    private DbSet<TEntity> DbSet { get; } = context.Set<TEntity>();

    private static EntityQueryOptions<TEntity>? _fallbackQueryOptions;

    private readonly EntityQueryOptions<TEntity> _queryOptions =
        options?.QueryOptions ?? (_fallbackQueryOptions ??= new EntityQueryOptions<TEntity>());

    private static EntityUpdateOptions<TEntity>? _fallbackUpdateOptions;

    private readonly EntityUpdateOptions<TEntity> _updateOptions =
        options?.UpdateOptions ?? (_fallbackUpdateOptions ??= new EntityUpdateOptions<TEntity>());

    private Task ValidateAsync(TEntity entity)
    {
        return options?.Validator?.ValidateAndThrowOnErrorsAsync(entity) ?? Task.CompletedTask;
    }

    public async Task<TEntity> InsertAsync(
        TEntity entity,
        bool autoSave = true,
        CancellationToken stoppingToken = default
    )
    {
        var inserted = DbSet.Add(entity).Entity;

        await ValidateAsync(inserted);
        if (autoSave)
            await SaveChangesAsync(stoppingToken);

        return inserted;
    }

    public async Task<TEntity> UpsertAsync(
        TEntity entity,
        Expression<Func<TEntity, bool>> on,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var upsert = await Queryable().FirstOrDefaultAsync(on, cancellationToken);
        if (upsert is not null)
            _updateOptions.Run(entity, upsert);
        else
            upsert = await InsertAsync(entity, false, cancellationToken);

        await ValidateAsync(upsert);
        if (autoSave)
            await SaveChangesAsync(cancellationToken);

        return upsert;
    }

    private static Guid? NullIfEmptyGuid(Guid? guid)
    {
        return guid == Guid.Empty ? null : guid;
    }

    public async Task<int> UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default
    )
    {
        await ValidateAsync(entity);
        return await SaveChangesAsync(cancellationToken);
    }

    private async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.Now;
        var currentUser = options?.CurrentUser;

        foreach (var entry in context.ChangeTracker.Entries())
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity is IAuditable insertAudit)
                    {
                        insertAudit.CreatedAt = now;
                        insertAudit.CreatedById = NullIfEmptyGuid(currentUser?.Id);
                    }

                    if (
                        options?.CurrentTenant is not null
                        && entry.Entity is IMultiTenant multiTenant
                    )
                        multiTenant.TenantId =
                            options.CurrentTenant.Id
                            ?? throw new AppUnauthorizedException("You are not in any tenant");

                    break;
                case EntityState.Modified:
                    if (entry.Entity is IAuditable updateAudit)
                    {
                        updateAudit.UpdatedAt = now;
                        updateAudit.UpdatedById = NullIfEmptyGuid(currentUser?.Id);
                    }

                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                default:
                    break;
            }

        return await context.SaveChangesAsync(cancellationToken);
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
        Expression<Func<TEntity, bool>>? predicate = null,
        bool isIncludeDetails = false,
        CancellationToken cancellationToken = default
    )
    {
        if (predicate == null)
            return await Queryable(isIncludeDetails).ToListAsync(cancellationToken);

        return await Queryable(isIncludeDetails).Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    )
    {
        return await DbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    )
    {
        return await DbSet.CountAsync(predicate, cancellationToken);
    }

    public IPaginateOrderBuilding<TEntity> Paginate(
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? filter = null
    )
    {
        var queryable = filter is not null ? filter(DbSet) : DbSet;
        return new PaginateQueryBuilder<TEntity>(() => queryable.CountAsync(), queryable);
    }

    public IQueryable<TEntity> Queryable(bool isIncludeDetails = false)
    {
        var queryable = _queryOptions.DefaultIncludeQuery.Invoke(DbSet);
        if (isIncludeDetails)
            queryable = _queryOptions.IncludeDetailsQuery.Invoke(queryable);

        return queryable;
    }
}
