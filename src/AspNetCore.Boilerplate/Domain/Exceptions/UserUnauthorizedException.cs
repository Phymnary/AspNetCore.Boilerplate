using System.Net;

namespace AspNetCore.Boilerplate.Domain;

public class UserUnauthorizedException(string message) : DomainException(message)
{
    public override HttpStatusCode StatusCode { get; } = HttpStatusCode.Unauthorized;
}
