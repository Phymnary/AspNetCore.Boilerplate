using System.Collections.Immutable;
using AspNetCore.Boilerplate.Roslyn.Constants;
using AspNetCore.Boilerplate.Roslyn.Extensions;
using AspNetCore.Boilerplate.Roslyn.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace AspNetCore.Boilerplate.Roslyn.Components.Endpoint;

public partial class EndpointGenerator
{
    private static readonly string[] Methods =
    {
        "Get",
        "Put",
        "Post",
        "Delete",
        "Head",
        "Options",
        "Trace",
        "Patch",
        "Connect",
    };

    private static readonly string[] CompareMethods = Methods
        .Select(it => "." + it.ToLower())
        .ToArray();

    private static class Execute
    {
        public static bool TryGetConfigRouteMethodInfo(
            ISymbol symbol,
            out ConfigRouteMethodInfo info
        )
        {
            var containingType = symbol.ContainingType;
            var namespaces = containingType
                .ContainingNamespace.ToDisplayString(
                    new SymbolDisplayFormat(
                        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces
                    )
                )
                .Split('.')
                .ToImmutableArray();

            info = new ConfigRouteMethodInfo(
                containingType.GetFullyQualifiedName(),
                symbol.Name,
                namespaces
            );

            return true;
        }

        public static bool TryGetEndpointInfo(
            INamedTypeSymbol symbol,
            ClassDeclarationSyntax classSyntax,
            AttributeData attribute,
            CancellationToken token,
            out EndpointInfo? info
        )
        {
            var methodName = string.Empty;
            var hierarchy = HierarchyInfo.From(symbol);

            token.ThrowIfCancellationRequested();

            if (attribute.ConstructorArguments.Length > 0)
            {
                methodName = attribute.ConstructorArguments[0].ToCSharpString();
                methodName = methodName.Substring(methodName.LastIndexOf('.') + 1);
            }
            else
            {
                var lowerNs = hierarchy.Namespace.ToLower();

                foreach (var method in CompareMethods.Where(method => lowerNs.EndsWith(method)))
                {
                    methodName = Methods[Array.IndexOf(CompareMethods, method)];
                    break;
                }

                if (methodName == string.Empty)
                {
                    info = null;
                    return false;
                }
            }

            token.ThrowIfCancellationRequested();

            var hasProperty = classSyntax.Members.Any(member =>
                member is PropertyDeclarationSyntax { Identifier.ValueText: "RoutePattern" }
            );

            var hasMethod = classSyntax.Members.Any(member =>
                member is MethodDeclarationSyntax { Identifier.ValueText: "BuildRoute" }
            );

            token.ThrowIfCancellationRequested();

            info = new EndpointInfo(
                methodName,
                "HandleAsync",
                symbol.GetFullyQualifiedName(),
                hasProperty ? "RoutePattern" : string.Empty,
                hasMethod ? "BuildRoute" : string.Empty,
                hierarchy.Namespace.Split('.').ToImmutableArray(),
                hierarchy
            );

            token.ThrowIfCancellationRequested();

            return true;
        }

        private static T? FindClosest<T>(EndpointInfo endpoint, ImmutableArray<T> methodInfos)
            where T : IConfigRouteMethodInfo
        {
            T? selected = default;
            var highest = -1;

            foreach (var methodInfo in methodInfos)
            {
                int i;
                for (i = 0; i < methodInfo.Namespaces.Length && i < endpoint.Namespaces.Length; i++)
                {
                    if (methodInfo.Namespaces[i] != endpoint.Namespaces[i])
                        break;
                }

                if (
                    i > highest
                    || (i == highest && methodInfo.Namespaces.Length < selected!.Namespaces.Length)
                )
                {
                    highest = i;
                    selected = methodInfo;
                }
            }

            return selected;
        }

        public static ConfigRouteMethodInfo? FindClosestPattern(
            EndpointInfo endpoint,
            ImmutableArray<ConfigRouteMethodInfo> patterns
        )
        {
            return FindClosest(endpoint, patterns);
        }

        public static ConfigRouteMethodInfo? FindClosestBuilder(
            EndpointInfo endpoint,
            ImmutableArray<ConfigRouteMethodInfo> builders
        )
        {
            return FindClosest(endpoint, builders);
        }

        public static LocalDeclarationStatementSyntax? GetMapRouteMethodExpression(
            EndpointInfo info,
            ConfigRouteMethodInfo? patternInfo
        )
        {
            ArgumentSyntax argument;
            if (info.RoutePatternPropertyName != string.Empty)
                argument = Argument(IdentifierName("RoutePattern"));
            else if (patternInfo is not null)
                argument = Argument(
                    InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName(patternInfo.ContainingType),
                                IdentifierName(patternInfo.MethodName)
                            )
                        )
                        .WithArgumentList(
                            ArgumentList(SingletonSeparatedList(Argument(ThisExpression())))
                        )
                );
            else
                return null;

            var invocation = InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(
                            "global::Microsoft.AspNetCore.Builder.EndpointRouteBuilderExtensions"
                        ),
                        IdentifierName($"Map{info.HttpMethod}")
                    )
                )
                .WithArgumentList(
                    ArgumentList(
                        SeparatedList<ArgumentSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                Argument(IdentifierName("app")),
                                Token(SyntaxKind.CommaToken),
                                argument,
                                Token(SyntaxKind.CommaToken),
                                Argument(IdentifierName(info.HandleMethodName)),
                            }
                        )
                    )
                );

            return LocalDeclarationStatement(
                VariableDeclaration(IdentifierName("var"))
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(Identifier("builder"))
                                .WithInitializer(EqualsValueClause(invocation))
                        )
                    )
            );
        }

        public static ExpressionStatementSyntax? GetBuildRouteMethodExpression(
            EndpointInfo info,
            ConfigRouteMethodInfo? builderInfo
        )
        {
            InvocationExpressionSyntax invocation;
            if (info.BuildRouteMethodName != string.Empty)
                invocation = InvocationExpression(IdentifierName(info.BuildRouteMethodName));
            else if (builderInfo is not null)
                invocation = InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(builderInfo.ContainingType),
                        IdentifierName(builderInfo.MethodName)
                    )
                );
            else
                return null;

            return ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    IdentifierName("builder"),
                    invocation.WithArgumentList(
                        ArgumentList(SingletonSeparatedList(Argument(IdentifierName("builder"))))
                    )
                )
            );
        }

        public static ExpressionStatementSyntax GetMapEndpointExpression(EndpointInfo endpoint)
        {
            return ExpressionStatement(
                InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(
                                $"{GeneratorConstant.Global}AspNetCore.Boilerplate.Api.Extensions.EndpointRouteBuilderExtensions"
                            ),
                            GenericName("MapEndpoint")
                                .WithTypeArgumentList(
                                    TypeArgumentList(
                                        SingletonSeparatedList<TypeSyntax>(
                                            IdentifierName(endpoint.TypeName)
                                        )
                                    )
                                )
                        )
                    )
                    .WithArgumentList(
                        ArgumentList(SingletonSeparatedList(Argument(IdentifierName("app"))))
                    )
            );
        }
    }
}
