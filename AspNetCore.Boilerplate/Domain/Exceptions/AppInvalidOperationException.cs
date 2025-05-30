using System.Net;

namespace AspNetCore.Boilerplate.Domain;

public class AppInvalidOperationException(string message) : Exception(message), IBusinessException
{
    public HttpStatusCode StatusCode => HttpStatusCode.UnprocessableEntity;
}
