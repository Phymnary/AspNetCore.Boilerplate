using System.Net;
using FluentValidation.Results;

namespace AspNetCore.Boilerplate.Domain;

public class EntityValidationException(string message) : Exception(message), IBusinessException
{
    public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    
    public string? ErrorCode { get; private set; }

    public List<ValidationFailure> Failures { get; init; } = [];
    
    public EntityValidationException WithErrorCode(string code)
    {
        ErrorCode = code;
        return this;
    }
}
