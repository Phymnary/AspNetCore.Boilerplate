using AspNetCore.Boilerplate.Domain;
using AspNetCore.Boilerplate.Extensions;
using FluentValidation;

namespace AspNetCore.Boilerplate.Application.Extensions;

public static class ValidatorExtensions
{
    public static async Task ValidateAndThrowOnErrorsAsync<T>(
        this IValidator<T> validator,
        T entity,
        CancellationToken token = default
    )
    {
        var result = await validator.ValidateAsync(entity, token);

        if (!result.IsValid)
            throw new EntityValidationException(result.Errors?.ToString() ?? "");
    }
}
