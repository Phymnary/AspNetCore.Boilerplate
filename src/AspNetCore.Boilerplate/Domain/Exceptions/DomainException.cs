using System.Net;

namespace AspNetCore.Boilerplate.Domain;

public abstract class DomainException(string message) : Exception(message)
{
    public abstract HttpStatusCode StatusCode { get; }
}
