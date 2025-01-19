using System.Linq.Expressions;
using AspNetCore.Boilerplate.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static System.Linq.Expressions.Expression;

namespace AspNetCore.Boilerplate.EntityFrameworkCore.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder BuildEntity<TEntity>(
        this ModelBuilder modelBuilder,
        Action<EntityTypeBuilder<TEntity>>? additionalConfigure = null,
        ICurrentTenant? currentTenant = null
    )
        where TEntity : class, IEntity
    {
        modelBuilder.Entity<TEntity>(b =>
        {
            b.ToTable(typeof(TEntity).Name);

            if (CreateQueryFilter<TEntity>(currentTenant) is { } queryFilter)
                b.HasQueryFilter(queryFilter);

            additionalConfigure?.Invoke(b);
        });
        return modelBuilder;
    }

    private static Expression<Func<TEntity, bool>>? CreateQueryFilter<TEntity>(
        ICurrentTenant? currentTenant = null
    )
        where TEntity : IEntity
    {
        var type = typeof(TEntity);
        var entity = Parameter(type, "entity");
        List<BinaryExpression> conditions = [];

        if (type.IsAssignableTo(typeof(IDeletable)))
            conditions.Add(Equal(Property(entity, nameof(IDeletable.DeletedAt)), Constant(null)));

        if (type.IsAssignableTo(typeof(IMultiTenant)) && currentTenant is not null)
            conditions.Add(
                Equal(
                    Property(entity, nameof(IMultiTenant.TenantId)),
                    Property(Constant(currentTenant), nameof(ICurrentTenant.Id))
                )
            );

        if (conditions.Count == 0)
            return null;

        BinaryExpression? predicate = null;
        conditions.ForEach(cond =>
        {
            predicate = predicate is null ? cond : And(predicate, cond);
        });

        return Lambda<Func<TEntity, bool>>(predicate!, entity);
    }
}
