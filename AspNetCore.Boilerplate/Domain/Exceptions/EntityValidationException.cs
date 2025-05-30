using System.Net;

namespace AspNetCore.Boilerplate.Domain;

public class EntityValidationException(string message) : Exception(message), IBusinessException
{
    public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
