using System.Linq.Expressions;
using AspNetCore.Boilerplate.Application.Extensions;
using AspNetCore.Boilerplate.Domain;
using AspNetCore.Boilerplate.Domain.Pagination;
using AspNetCore.Boilerplate.Domain.ReadonlyQueries;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Boilerplate.EntityFrameworkCore;

public abstract class EfRepository<TDbContext, TEntity>(
    TDbContext context,
    EfRepositoryAddons? addons = null,
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

    private CancellationToken GetRequestAborted(CancellationToken token)
    {
        return token == default ? addons?.CancellationTokenProvider.Get() ?? default : token;
    }

    private Task ValidateAsync(TEntity entity, CancellationToken token)
    {
        return options?.Validator?.ValidateAndThrowOnErrorsAsync(
                entity,
                $"Fail when validate {typeof(TEntity).Name}",
                token
            ) ?? Task.CompletedTask;
    }

    public async Task<TEntity> InsertAsync(
        TEntity entity,
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        var ct = GetRequestAborted(cancellationToken);
        var inserted = DbSet.Add(entity).Entity;

        await ValidateAsync(inserted, ct);
        if (autoSave)
            await SaveChangesAsync(ct);

        return inserted;
    }

    public async Task<TEntity> UpsertAsync(
        TEntity entity,
        Expression<Func<TEntity, bool>> on,
        bool autoSave = true,
        bool isIncludeDetails = false,
        CancellationToken cancellationToken = default
    )
    {
        var ct = GetRequestAborted(cancellationToken);
        var upsert = await Queryable(isIncludeDetails).FirstOrDefaultAsync(on, ct);
        if (upsert is not null)
            _updateOptions.Run(entity, upsert);
        else
            upsert = await InsertAsync(entity, false, ct);

        await ValidateAsync(upsert, ct);
        if (autoSave)
            await SaveChangesAsync(ct);

        return upsert;
    }

    public async Task<int> UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default
    )
    {
        var ct = GetRequestAborted(cancellationToken);
        await ValidateAsync(entity, ct);
        return await SaveChangesAsync(ct);
    }

    private async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        if (addons?.SaveChangesInterceptors is not null)
            foreach (var interceptor in addons.SaveChangesInterceptors)
                await interceptor.RunAsync(cancellationToken);

        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<TEntity?> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool isIncludeDetails = false,
        CancellationToken cancellationToken = default
    )
    {
        var ct = GetRequestAborted(cancellationToken);
        return await Queryable(isIncludeDetails).FirstOrDefaultAsync(predicate, ct);
    }

    public async Task<List<TEntity>> QueryAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        bool isIncludeDetails = false,
        CancellationToken cancellationToken = default
    )
    {
        var ct = GetRequestAborted(cancellationToken);
        if (predicate == null)
            return await Queryable(isIncludeDetails).ToListAsync(ct);

        return await Queryable(isIncludeDetails).Where(predicate).ToListAsync(ct);
    }

    public async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    )
    {
        var ct = GetRequestAborted(cancellationToken);
        return await DbSet.AnyAsync(predicate, ct);
    }

    public async Task<int> CountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    )
    {
        var ct = GetRequestAborted(cancellationToken);
        return await DbSet.CountAsync(predicate, ct);
    }

    public IPaginateOrderBuilding<TEntity> Paginate(
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? filter = null,
        CancellationToken cancellationToken = default
    )
    {
        var ct = GetRequestAborted(cancellationToken);
        var queryable = filter is not null ? filter(DbSet) : DbSet;
        return new PaginateQueryBuilder<TEntity>(() => queryable.CountAsync(ct), queryable);
    }

    public IQueryable<TEntity> Queryable(bool isIncludeDetails = false)
    {
        var queryable = _queryOptions.DefaultIncludeQuery.Invoke(DbSet);
        if (isIncludeDetails)
            queryable = _queryOptions.IncludeDetailsQuery.Invoke(queryable);

        return queryable;
    }

    public ReadonlyQuery<TEntity> ReadonlyQuery(
        Expression<Func<TEntity, bool>> predicate,
        bool? isIncludeDetails = null
    )
    {
        var queryable = isIncludeDetails is { } useIncludeOptions
            ? Queryable(useIncludeOptions)
            : DbSet;

        return new ReadonlyQuery<TEntity>(queryable.Where(predicate).AsNoTracking());
    }
}
