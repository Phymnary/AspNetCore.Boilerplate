using System.Net;

namespace AspNetCore.Boilerplate.Domain;

public class EntityValidationException(string message) : DomainException(message)
{
    public override HttpStatusCode StatusCode => HttpStatusCode.UnprocessableEntity;
}
