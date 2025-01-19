using System.Collections.Immutable;
using AspNetCore.Boilerplate.Roslyn.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace AspNetCore.Boilerplate.Roslyn.Components.Dependency;

public partial class DependencyGenerator
{
    private static class Execute
    {
        public static bool TryGetDependencyInfo(
            INamedTypeSymbol typeSymbol,
            ImmutableArray<AttributeData> matchedAttributes,
            CancellationToken token,
            out DependencyInfo? dependencyInfo
        )
        {
            if (typeSymbol is not { IsAbstract: false, IsGenericType: false })
            {
                dependencyInfo = null;
                return false;
            }

            string serviceTypeName;
            var typeName = typeSymbol.GetFullyQualifiedName();
            var attribute = matchedAttributes.First();

            if (attribute.GetNamedArgument("IsSelf") is true)
                serviceTypeName = typeName;
            else
                serviceTypeName =
                    typeSymbol.Interfaces.Length > 0
                        ? typeSymbol.Interfaces[0].GetFullyQualifiedName()
                        : typeName;

            if (attribute.ConstructorArguments.Length == 0)
            {
                dependencyInfo = null;
                return false;
            }

            token.ThrowIfCancellationRequested();

            var lifetime = attribute.ConstructorArguments.FirstOrDefault();
            var fullyQualifiedLifetime = lifetime.ToCSharpString();

            dependencyInfo = new DependencyInfo(
                serviceTypeName,
                typeName,
                fullyQualifiedLifetime.Substring(fullyQualifiedLifetime.LastIndexOf('.') + 1)
            );

            return true;
        }

        public static bool TryGetModuleErrors(
            ClassDeclarationSyntax nodeSyntax,
            CancellationToken _
        )
        {
            return nodeSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);
        }

        public static ExpressionStatementSyntax GetRegistrationExpression(DependencyInfo info)
        {
            return ExpressionStatement(
                InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("services"),
                        GenericName(Identifier($"Add{info.Lifetime}"))
                            .WithTypeArgumentList(
                                TypeArgumentList(
                                    SeparatedList<TypeSyntax>(
                                        new SyntaxNodeOrToken[]
                                        {
                                            IdentifierName(info.ServiceTypeName),
                                            Token(SyntaxKind.CommaToken),
                                            IdentifierName(info.ImplementationTypeName),
                                        }
                                    )
                                )
                            )
                    )
                )
            );
        }
    }
}