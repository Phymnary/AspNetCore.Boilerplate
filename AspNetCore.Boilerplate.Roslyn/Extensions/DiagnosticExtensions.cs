using AspNetCore.Boilerplate.Roslyn.Helper;
using AspNetCore.Boilerplate.Roslyn.Models;
using Microsoft.CodeAnalysis;

namespace AspNetCore.Boilerplate.Roslyn.Extensions;

internal static class DiagnosticExtensions
{
    public static void Add(
        this in ImmutableArrayBuilder<DiagnosticInfo> diagnostics,
        DiagnosticDescriptor descriptor,
        ISymbol symbol,
        params object[] args
    )
    {
        diagnostics.Add(DiagnosticInfo.Create(descriptor, symbol, args));
    }
}