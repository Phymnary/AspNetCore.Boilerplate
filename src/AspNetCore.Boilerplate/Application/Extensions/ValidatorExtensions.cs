using AspNetCore.Boilerplate.Domain;
using FluentValidation;

namespace AspNetCore.Boilerplate.Application.Extensions;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<TEntity, TProperty> WhenCreating<TEntity, TProperty>(
        this IRuleBuilderOptions<TEntity, TProperty> rule
    )
        where TEntity : IEntity, IAuditable
    {
        return rule.When(entity => entity.CreatedAt == default);
    }

    public static IRuleBuilderOptions<TEntity, TProperty> WhenUpdating<TEntity, TProperty>(
        this IRuleBuilderOptions<TEntity, TProperty> rule
    )
        where TEntity : IEntity, IAuditable
    {
        return rule.When(entity => entity.CreatedAt != default);
    }
}
