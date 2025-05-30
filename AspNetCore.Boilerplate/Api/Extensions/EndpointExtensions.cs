using AspNetCore.Boilerplate.Extensions;

namespace AspNetCore.Boilerplate.Api.Extensions;

public static class EndpointExtensions
{
    public static string GetRoutePatternBasedOnNamespace<TEndpoint>(
        this TEndpoint _,
        string rootNamespace,
        string prefix = ""
    )
        where TEndpoint : IEndpoint
    {
        var ns = typeof(TEndpoint).Namespace!.RemoveSurFix(rootNamespace);

        ns = ns.RemovePostFix(".POST")
            .RemovePostFix(".GET")
            .RemovePostFix(".DELETE")
            .RemovePostFix(".PATCH")
            .RemovePostFix(".PUT");

        var routeName = string.Join(
            "/",
            ns.Split(".", StringSplitOptions.RemoveEmptyEntries)
                .Select(static item =>
                {
                    var path = item;
                    var isDynamic = false;

                    if (item.StartsWith('_') && item.EndsWith('_'))
                    {
                        isDynamic = true;
                        path = path[1..^1];
                    }

                    path = path.PascalToKebabCase().ToLower();
                    return isDynamic ? "{" + path + "}" : path;
                })
        );

        return $"{prefix}/{routeName}";
    }
}
