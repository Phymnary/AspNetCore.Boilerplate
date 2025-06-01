using AspNetCore.Boilerplate.Domain;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Boilerplate.EntityFrameworkCore.Interceptors;

public class MultiTenantSaveChangesInterceptor<TDbContext>(
    TDbContext dbContext,
    ICurrentTenant currentTenant
) : IEfSaveChangesInterceptor
    where TDbContext : DbContext
{
    public ValueTask RunAsync(CancellationToken _)
    {
        foreach (var entry in dbContext.ChangeTracker.Entries<IMultiTenant>())
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.TenantId =
                        currentTenant.Id
                        ?? throw new AspUnauthorizedException("You are not in any tenant");
                    break;
                case EntityState.Modified:
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                default:
                    break;
            }

        return default;
    }
}
