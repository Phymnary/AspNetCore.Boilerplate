using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.Boilerplate.Domain.Default;

public class DefaultCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public Guid? Id
    {
        get
        {
            var id = accessor.HttpContext?.User.FindFirstValue("sub");

            if (string.IsNullOrEmpty(id))
                return null;

            return new Guid(id);
        }
    }
}
