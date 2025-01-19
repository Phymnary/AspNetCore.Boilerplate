using Microsoft.AspNetCore.Builder;

namespace AspNetCore.Boilerplate.Api;

public interface IExtendRouteBuilder
{
    RouteHandlerBuilder Extend(RouteHandlerBuilder routeBuilder);
}
