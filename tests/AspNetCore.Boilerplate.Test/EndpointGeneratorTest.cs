using AspNetCore.Boilerplate.Api;
using AspNetCore.Boilerplate.Roslyn.Components.Endpoint;
using Microsoft.AspNetCore.Routing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace AspNetCore.Boilerplate.Test;

public class ApiControllerGeneratorTest
{
    private const string ClassText =
        @"
using System;

namespace TestNamespace.SourceGenerator.GET
{
    [global::AspNetCore.Boilerplate.Api.Controller]
    public partial class ApiController
    {
    }

    public static class ApiGroup
    {
        [global::AspNetCore.Boilerplate.Api.RoutePattern]
        public static string GetRouteName<TEndpoint>(TEndpoint endpoint, string? prefix = null)
            where TEndpoint : class, global::AspNetCore.Boilerplate.Api.IEndpoint
        {
            return "";
        }

        [global::AspNetCore.Boilerplate.Api.RouteBuilder]
         public static void BuildRoute(Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
        {
            builder.RequireAuthorization();
        }
    }
}
";
    
    private const string Other =
        @"
using System;

namespace TestNamespace.SourceGenerator.Connect.GET
{
    public static class ConnectGroup
    {
        [global::AspNetCore.Boilerplate.Api.RoutePattern]
        public string GetRouteName<TEndpoint>(TEndpoint endpoint, string? prefix = null)
            where TEndpoint : class, global::AspNetCore.Boilerplate.Api.IEndpoint
        {
            return "";
        }
    }

    [global::AspNetCore.Boilerplate.Api.Endpoint]
    public partial class TestEndpoint
    {
        public int HandleAsync() { return 1; } 
    }
}
";

    [Fact]
    public void Generate()
    {
        var generator = new EndpointGenerator();

        var driver = CSharpGeneratorDriver.Create(generator);

        // We need to create a compilation with the required source code.
        var compilation = CSharpCompilation.Create(
            nameof(ApiControllerGeneratorTest),
            [CSharpSyntaxTree.ParseText(ClassText), CSharpSyntaxTree.ParseText(Other)],
            [
                // To support 'System.Attribute' inheritance, add reference to 'System.Private.CoreLib'.
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IEndpoint).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(EndpointAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IEndpointRouteBuilder).Assembly.Location),
            ]
        );

        var runResult = driver.RunGenerators(compilation).GetRunResult();
        var generatedFileSyntax = runResult.GeneratedTrees.Single(t =>
            t.FilePath.EndsWith("TestEndpoint.g.cs")
        );

        var generatedControllerSyntax = runResult.GeneratedTrees.Single(t =>
            t.FilePath.EndsWith("ApiController.g.cs")
        );

        var text = generatedFileSyntax.GetText().ToString();
        var text2 = generatedControllerSyntax.GetText().ToString();
    }
}