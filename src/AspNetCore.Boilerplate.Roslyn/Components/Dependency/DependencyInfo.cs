namespace AspNetCore.Boilerplate.Roslyn.Components.Dependency;

internal sealed record DependencyInfo(
    string ServiceTypeName,
    string ImplementationTypeName,
    string Lifetime
);