using AspNetCore.Boilerplate.Domain;
using AspNetCore.Boilerplate.Extensions;
using FluentValidation;

namespace AspNetCore.Boilerplate.Application.Extensions;

public static class ValidatorExtensions
{
    public static async Task ValidateAndThrowOnErrorsAsync<T>(
        this IValidator<T> validator,
        T entity,
        string message,
        CancellationToken token = default
    )
    {
        var result = await validator.ValidateAsync(entity, token);

        if (!result.IsValid)
            throw new EntityValidationException(message)
            {
                Failures = result.Errors,
            };
    }
}
