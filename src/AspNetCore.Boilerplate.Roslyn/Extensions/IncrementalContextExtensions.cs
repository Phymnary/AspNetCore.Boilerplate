using AspNetCore.Boilerplate.Roslyn.Helper;
using AspNetCore.Boilerplate.Roslyn.Models;
using Microsoft.CodeAnalysis;

namespace AspNetCore.Boilerplate.Roslyn.Extensions;

internal static class IncrementalContextExtensions
{
    public static void ReportDiagnostics(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<EquatableArray<DiagnosticInfo>> diagnostics
    )
    {
        context.RegisterSourceOutput(
            diagnostics,
            static (context, diagnostics) =>
            {
                foreach (var diagnostic in diagnostics)
                    context.ReportDiagnostic(diagnostic.ToDiagnostic());
            }
        );
    }

    public static void ReportDiagnostics(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<Diagnostic> diagnostics
    )
    {
        context.RegisterSourceOutput(
            diagnostics,
            static (context, diagnostic) => context.ReportDiagnostic(diagnostic)
        );
    }
}