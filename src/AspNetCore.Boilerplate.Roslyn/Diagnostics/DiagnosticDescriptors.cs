using AspNetCore.Boilerplate.Roslyn.Components.Dependency;
using Microsoft.CodeAnalysis;

namespace AspNetCore.Boilerplate.Roslyn.Diagnostics;

internal static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor InvalidClassDecorationForDependency =
        new(
            "ANCB0001",
            "Invalid Class Decoration for [Dependency] to be registered",
            "The Dependency {0} could not be registered as a dependency because it's a abstract or generic class",
            nameof(DependencyGenerator),
            DiagnosticSeverity.Error,
            true,
            "Class annotated with [Dependency] must not be abstract or generic.",
            ""
        );

    public static readonly DiagnosticDescriptor ModuleMustBePartialForAuto =
        new(
            "ANCB0002",
            "Module must be partial",
            "The Module {0} has the [Auto] attribute must be marked as partial",
            nameof(DependencyGenerator),
            DiagnosticSeverity.Error,
            true,
            "Module class must be marked as partial.",
            ""
        );
}