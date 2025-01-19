using AspNetCore.Boilerplate.Domain;
using AspNetCore.Boilerplate.Extensions;
using FluentValidation.Results;

namespace AspNetCore.Boilerplate.Application.Extensions;

public static class ValidationResultExtensions
{
    public static void ThrowOnErrors(this ValidationResult result)
    {
        if (!result.IsValid)
            throw new EntityValidationException(result.Errors.JoinAsString(", "));
    }
}
