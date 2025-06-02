using System.Net;
using System.Net.Mime;
using System.Text.Json;
using AspNetCore.Boilerplate.Domain;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace AspNetCore.Boilerplate.Api;

internal class AspExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        HttpStatusCode status;
        string? errorCode = null;
        List<ValidationFailure>? validationErrors = null;

        switch (exception)
        {
            case IBusinessException businessException:
                status = businessException.StatusCode;
                errorCode = businessException.ErrorCode;
                break;
            case DbUpdateException:
                status = HttpStatusCode.FailedDependency;
                break;
            default:
                status = HttpStatusCode.InternalServerError;
                break;
        }

        httpContext.Response.StatusCode = (int)status;
        httpContext.Response.ContentType = MediaTypeNames.Application.Json;
        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(
                new AspErrorMessage
                {
                    Error = new AspErrorDto
                    {
                        Message = exception.Message,
                        Details = errorCode,
                        ValidationErrors = validationErrors?.Select(
                            failure => new AspValidationErrorMessageDto
                            {
                                Message = failure.ErrorMessage,
                                Property = failure.PropertyName,
                            }
                        ),
                    },
                },
                AspJsonSerializerContext.Default.AspErrorMessage
            ),
            cancellationToken
        );

        return true;
    }
}
