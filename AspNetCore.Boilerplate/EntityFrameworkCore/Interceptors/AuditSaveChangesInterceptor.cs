using System.Text.Json;
using AspNetCore.Boilerplate.Domain;
using AspNetCore.Boilerplate.Domain.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AspNetCore.Boilerplate.EntityFrameworkCore.Interceptors;

public class AuditSaveChangesInterceptor<TDbContext>(TDbContext dbContext, ICurrentUser currentUser)
    : IEfSaveChangesInterceptor
    where TDbContext : DbContext
{
    private static Guid? NullIfEmptyGuid(Guid? guid)
    {
        return guid == Guid.Empty ? null : guid;
    }

    public ValueTask RunAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in dbContext.ChangeTracker.Entries<IAuditable>())
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedById = NullIfEmptyGuid(currentUser?.Id);
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedById = NullIfEmptyGuid(currentUser?.Id);

                    TrackEntityPropertyChange(entry);
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                default:
                    break;
            }

        return default;
    }

    private void TrackEntityPropertyChange(EntityEntry<IAuditable> entry)
    {
        if (!AuditingMetadata.HasEntityPropertyChange)
            return;

        var changes =
            from property in entry.Properties
            where property.IsModified
            select new EntityPropertyChange
            {
                Entity = entry.Metadata.GetSchemaQualifiedTableName()!,
                NewValue = JsonSerializer.Serialize(property.CurrentValue),
                OriginalValue = JsonSerializer.Serialize(property.OriginalValue),
                PropertyName = property.Metadata.Name,
                PropertyTypeFullName = property.Metadata.ClrType.FullName ?? "",
            };

        dbContext.Set<EntityPropertyChange>().AddRange(changes);
    }
}
