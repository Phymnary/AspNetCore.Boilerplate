using Microsoft.AspNetCore.Http;

namespace AspNetCore.Boilerplate.Domain.Default;

public class DefaultCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public Guid? Id
    {
        get
        {
            var id = accessor.HttpContext?.User.FindFirst("sub");

            if (id != null)
                return new Guid(id.Value);

            return null;
        }
    }
}
