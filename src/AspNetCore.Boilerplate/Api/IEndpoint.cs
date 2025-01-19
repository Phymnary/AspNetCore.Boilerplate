using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace AspNetCore.Boilerplate.Api;

public interface IEndpoint
{
    RouteHandlerBuilder ConfigureRouteBuilder(IEndpointRouteBuilder app);
}
