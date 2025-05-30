using System.Net;

namespace AspNetCore.Boilerplate.Domain;

public class AppUnauthorizedException(string message) : Exception(message), IBusinessException
{
    public HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
}
