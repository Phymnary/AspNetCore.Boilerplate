using AspNetCore.Boilerplate.Domain;

namespace AspNetCore.Boilerplate.Api;

public class HttpContextCurrentTenant : ICurrentTenant
{
    public Guid? Id { get; set; }
}
