using System.Collections.Immutable;
using AspNetCore.Boilerplate.Roslyn.Constants;
using AspNetCore.Boilerplate.Roslyn.Extensions;
using AspNetCore.Boilerplate.Roslyn.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ReSharper disable SuggestVarOrType_Elsewhere

namespace AspNetCore.Boilerplate.Roslyn.Components.Dependency;

[Generator(LanguageNames.CSharp)]
public partial class DependencyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<DependencyInfo> dependencyInfos = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                $"{GeneratorConstant.Namespace}.DependencyAttribute",
                static (node, _) => node is ClassDeclarationSyntax,
                static (ctx, token) =>
                {
                    if (
                        !ctx.SemanticModel.Compilation.HasLanguageVersionAtLeastEqualTo(
                            LanguageVersion.CSharp8
                        )
                    )
                        return null;

                    var typeSymbol = (INamedTypeSymbol)ctx.TargetSymbol;

                    Execute.TryGetDependencyInfo(typeSymbol, ctx.Attributes, token, out var info);

                    return info;
                }
            )
            .Where(info => info is not null)!;

        IncrementalValuesProvider<HierarchyInfo> moduleHierarchies = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                $"{GeneratorConstant.Namespace}.AutoAttribute",
                static (node, _) => node is ClassDeclarationSyntax,
                static (ctx, token) =>
                {
                    _ = Execute.TryGetModuleHierarchy(
                        (ClassDeclarationSyntax)ctx.TargetNode,
                        (INamedTypeSymbol)ctx.TargetSymbol,
                        token,
                        out var info
                    );

                    return info;
                }
            )
            .Where(module => module is not null)!;

        IncrementalValuesProvider<(
            HierarchyInfo Hierarchy,
            ImmutableArray<DependencyInfo> Info
        )> grouped = moduleHierarchies.Combine(dependencyInfos.Collect());

        context.RegisterSourceOutput(
            grouped,
            static (src, item) =>
            {
                var registerExpressions = item
                    .Info.Select(Execute.GetRegistrationExpression)
                    .ToImmutableArray();

                var compilationUnit = BuildSyntax.GetCompilationUnitForDependency(
                    item.Hierarchy,
                    registerExpressions
                );

                src.AddSource($"{item.Hierarchy.FilenameHint}.g.cs", compilationUnit);
            }
        );
    }
}
