using System.Collections.Immutable;

namespace AspNetCore.Boilerplate.Roslyn.Components.Endpoint;

public record ConfigRouteMethodInfo(
    string ContainingType,
    string MethodName,
    ImmutableArray<string> Namespaces
) : IConfigRouteMethodInfo;