using AspNetCore.Boilerplate.Extensions;

namespace AspNetCore.Boilerplate.Api.Extensions;

public static class EndpointExtensions
{
    public static string GetRouteBasedOnNamespace<TEndpoint>(
        this TEndpoint _,
        string assemblyNamespace,
        string prefix
    )
        where TEndpoint : IEndpoint
    {
        var ns = typeof(TEndpoint).Namespace!.Replace(assemblyNamespace, string.Empty);

        ns = ns.RemovePostFix(".POST")
            .RemovePostFix(".GET")
            .RemovePostFix(".DELETE")
            .RemovePostFix(".PATCH")
            .RemovePostFix(".PUT");

        var routeName = string.Join(
            "/",
            ns.Replace(assemblyNamespace, string.Empty)
                .Split(".", StringSplitOptions.RemoveEmptyEntries)
                .Select(static item =>
                {
                    var workString = item;
                    var isDynamic = false;

                    if (item.StartsWith('_') && item.EndsWith('_'))
                    {
                        isDynamic = true;
                        workString = workString[1..^1];
                    }

                    workString = workString.PascalToKebabCase().ToLower();
                    return isDynamic ? "{" + workString + "}" : workString;
                })
        );

        return $"{prefix}/{routeName}";
    }
}
