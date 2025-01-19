using Microsoft.AspNetCore.Routing;

namespace AspNetCore.Boilerplate.Api.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : class, IEndpoint, new()
    {
        var endpoint = new TEndpoint();
        var builder = endpoint.ConfigureRouteBuilder(app);

        if (endpoint is IExtendRouteBuilder extendRouteBuilder)
        {
            builder = extendRouteBuilder.Extend(builder);
        }
    }
}
