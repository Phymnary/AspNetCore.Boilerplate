using System.Collections.Immutable;

namespace AspNetCore.Boilerplate.Roslyn.Components.Endpoint;

public record GroupConfigRouteMethodInfo(
    string ContainingType,
    string MethodName,
    ImmutableArray<string> Namespaces
);
