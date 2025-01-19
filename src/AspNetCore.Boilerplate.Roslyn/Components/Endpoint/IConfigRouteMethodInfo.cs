using System.Collections.Immutable;

namespace AspNetCore.Boilerplate.Roslyn.Components.Endpoint;

public interface IConfigRouteMethodInfo
{
    public string ContainingType { get; }
    public ImmutableArray<string> Namespaces { get; }
}
