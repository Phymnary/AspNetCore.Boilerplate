using AspNetCore.Boilerplate.Domain;
using AspNetCore.Boilerplate.Extensions;
using FluentValidation;

namespace AspNetCore.Boilerplate.Application.Extensions;

public static class ValidatorExtensions
{
    public static async Task ValidateAndThrowOnErrors<T>(this IValidator<T> validator, T entity)
    {
        var result = await validator.ValidateAsync(entity);

        if (!result.IsValid)
            throw new EntityValidationException(result.Errors.JoinAsString(". "));
    }
}
