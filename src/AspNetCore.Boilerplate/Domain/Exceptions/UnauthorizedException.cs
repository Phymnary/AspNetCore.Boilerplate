using System.Net;

namespace AspNetCore.Boilerplate.Domain;

public class UnauthorizedException(string message) : DomainException(message)
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
}
