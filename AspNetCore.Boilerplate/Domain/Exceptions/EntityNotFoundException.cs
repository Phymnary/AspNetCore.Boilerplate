using System.Net;

namespace AspNetCore.Boilerplate.Domain;

public class EntityNotFoundException(string message) : Exception(message), IBusinessException
{
    public HttpStatusCode StatusCode => HttpStatusCode.NotFound;
}
