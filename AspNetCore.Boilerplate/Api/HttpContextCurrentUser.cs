using AspNetCore.Boilerplate.Domain;

namespace AspNetCore.Boilerplate.Api;

public class HttpContextCurrentUser : ICurrentUser
{
    public Guid? Id { get; set; }
}
