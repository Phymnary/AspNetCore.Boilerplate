using Microsoft.AspNetCore.Http;

namespace AspNetCore.Boilerplate.Domain.Default;

public class DefaultCurrentTenant(IHttpContextAccessor accessor) : ICurrentTenant
{
    public Guid? Id
    {
        get
        {
            var id = accessor.HttpContext?.User.FindFirst("tenant");

            if (id != null)
                return new Guid(id.Value);

            return null;
        }
    }
}
