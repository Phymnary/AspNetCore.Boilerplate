using System.Net;

namespace AspNetCore.Boilerplate.Domain;

public class DbCommandException(string message) : DomainException(message)
{
    public override HttpStatusCode StatusCode => HttpStatusCode.FailedDependency;
}
