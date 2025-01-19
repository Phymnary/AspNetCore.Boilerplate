using System.Collections.Immutable;
using AspNetCore.Boilerplate.Roslyn.Constants;
using AspNetCore.Boilerplate.Roslyn.Extensions;
using AspNetCore.Boilerplate.Roslyn.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ReSharper disable SuggestVarOrType_Elsewhere

namespace AspNetCore.Boilerplate.Roslyn.Components.Endpoint;

[Generator(LanguageNames.CSharp)]
public partial class EndpointGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ConfigRouteMethodInfo> patternInfos =
            context.SyntaxProvider.ForAttributeWithMetadataName(
                $"{GeneratorConstant.ApiNamespace}.RoutePatternAttribute",
                static (node, _) =>
                    node is MethodDeclarationSyntax { Parent: ClassDeclarationSyntax },
                static (ctx, _) =>
                {
                    Execute.TryGetConfigRouteMethodInfo(ctx.TargetSymbol, out var info);

                    return info;
                }
            );

        IncrementalValuesProvider<ConfigRouteMethodInfo> builderInfos =
            context.SyntaxProvider.ForAttributeWithMetadataName(
                $"{GeneratorConstant.ApiNamespace}.RouteBuilderAttribute",
                static (node, _) =>
                    node is MethodDeclarationSyntax { Parent: ClassDeclarationSyntax },
                static (ctx, _) =>
                {
                    Execute.TryGetConfigRouteMethodInfo(ctx.TargetSymbol, out var info);

                    return info;
                }
            );

        IncrementalValuesProvider<EndpointInfo> endpointHierarchies = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                $"{GeneratorConstant.ApiNamespace}.EndpointAttribute",
                static (node, _) => node is ClassDeclarationSyntax,
                static (ctx, token) =>
                {
                    var typeSymbol = (INamedTypeSymbol)ctx.TargetSymbol;
                    var classSyntax = (ClassDeclarationSyntax)ctx.TargetNode;

                    if (ctx.Attributes.FirstOrDefault() is not { } attributeData)
                        return null;

                    var isSuccess = Execute.TryGetEndpointInfo(
                        typeSymbol,
                        classSyntax,
                        attributeData,
                        token,
                        out var info
                    );

                    return isSuccess ? info : null;
                }
            )
            .Where(it => it is not null)!;

        IncrementalValuesProvider<ControllerInfo> controllerHierarchies =
            context.SyntaxProvider.ForAttributeWithMetadataName(
                $"{GeneratorConstant.ApiNamespace}.ControllerAttribute",
                static (node, _) => node is ClassDeclarationSyntax,
                static (ctx, _) =>
                {
                    var classSymbol = (INamedTypeSymbol)ctx.TargetSymbol;

                    return new ControllerInfo(
                        classSymbol.IsStatic,
                        HierarchyInfo.From(classSymbol)
                    );
                }
            );

        IncrementalValuesProvider<(
            (EndpointInfo Endpoint, ImmutableArray<ConfigRouteMethodInfo> Patterns) Item,
            ImmutableArray<ConfigRouteMethodInfo> Builder
        )> endpointsWithPatterns = endpointHierarchies
            .Combine(patternInfos.Collect())
            .Combine(builderInfos.Collect());

        context.RegisterSourceOutput(
            endpointsWithPatterns,
            static (src, item) =>
            {
                var ((endpoint, patterns), builders) = item;

                ConfigRouteMethodInfo? closestPattern =
                    endpoint.RoutePatternPropertyName != string.Empty
                        ? null
                        : Execute.FindClosestPattern(endpoint, patterns);

                var methodExpression = Execute.GetMapRouteMethodExpression(
                    endpoint,
                    closestPattern
                );

                if (methodExpression is null)
                    return;

                var buildRouteExpression = Execute.GetBuildRouteMethodExpression(
                    endpoint,
                    Execute.FindClosestBuilder(endpoint, builders)
                );

                var compilationUnit = endpoint.Hierarchy.GetCompilationUnitForEndpoint(
                    methodExpression,
                    buildRouteExpression
                );

                src.AddSource($"{endpoint.Hierarchy.FilenameHint}.g.cs", compilationUnit);
            }
        );

        IncrementalValuesProvider<(
            ControllerInfo Controller,
            ImmutableArray<EndpointInfo> Endpoints
        )> controllerWithEndpoints = controllerHierarchies.Combine(endpointHierarchies.Collect());

        context.RegisterSourceOutput(
            controllerWithEndpoints,
            static (src, item) =>
            {
                var expressions = item
                    .Endpoints.Select(Execute.GetMapEndpointExpression)
                    .ToImmutableArray();

                var compilationUnit = item.Controller.Hierarchy.GetCompilationUnitForController(
                    item.Controller.IsStatic,
                    expressions
                );

                src.AddSource($"{item.Controller.Hierarchy.FilenameHint}.g.cs", compilationUnit);
            }
        );
    }
}
