using AspNetCore.Boilerplate.Roslyn.Models;

namespace AspNetCore.Boilerplate.Roslyn.Components.Endpoint;

internal record ControllerInfo(bool IsStatic, HierarchyInfo Hierarchy);