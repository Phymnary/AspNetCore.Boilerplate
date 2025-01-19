using System.Collections.Immutable;
using AspNetCore.Boilerplate.Roslyn.Models;

namespace AspNetCore.Boilerplate.Roslyn.Components.Endpoint;

internal record EndpointInfo(
    string HttpMethod,
    string HandleMethodName,
    string TypeName,
    string RoutePatternPropertyName,
    string BuildRouteMethodName,
    ImmutableArray<string> Namespaces,
    HierarchyInfo Hierarchy
);