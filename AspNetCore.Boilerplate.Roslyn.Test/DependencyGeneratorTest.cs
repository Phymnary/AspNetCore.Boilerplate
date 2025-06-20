﻿using AspNetCore.Boilerplate.Roslyn.Components.Dependency;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AspNetCore.Boilerplate.Test;

public sealed class DependencyGeneratorTest
{
    private const string ClassText = """
        using System;
        using Microsoft.Extensions.DependencyInjection;
        using AspNetCore.Boilerplate;

        namespace TestNamespace
        {
            [global::AspNetCore.Boilerplate.Dependency(global::AspNetCore.Boilerplate.Lifetime.Scoped)]
            public class FooService {}
        }
        """;

    private const string ModuleText = """
        using System;
        using Microsoft.Extensions.DependencyInjection;
        using AspNetCore.Boilerplate;

        namespace TestNamespace 
        {
            [Auto]
            public partial class SampleModule : IModule
            {
                public void ConfigureServices(IServiceCollection services) {}
            }
        }
        """;

    private const string Expected = """
        // <auto-generated/>
        #pragma warning disable
        #nullable enable
        namespace TestNamespace
        {
            partial class SampleModule : AspNetCore.Boilerplate.IAutoRegister
            {
                public void AddDependencies(IServiceCollection services)
                {
                }
            }
        }
        """;

    [Fact]
    public void generate_register_dependency_methods_for_module()
    {
        // Create an instance of the source generator.
        var generator = new DependencyGenerator();

        // Source generators should be tested using 'GeneratorDriver'.
        var driver = CSharpGeneratorDriver.Create(generator);

        // We need to create a compilation with the required source code.
        var compilation = CSharpCompilation.Create(
            nameof(DependencyGeneratorTest),
            [CSharpSyntaxTree.ParseText(ClassText), CSharpSyntaxTree.ParseText(ModuleText)],
            [
                // To support 'System.Attribute' inheritance, add reference to 'System.Private.CoreLib'.
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IModule).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(AutoAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IServiceCollection).Assembly.Location),
            ]
        );

        var runResult = driver.RunGenerators(compilation).GetRunResult();
        var generatedFileSyntax = runResult
            .GeneratedTrees.Single(t => t.FilePath.EndsWith("SampleModule.g.cs"))
            .GetText()
            .ToString();

        Assert.Equal(Expected, generatedFileSyntax, ignoreLineEndingDifferences: true);
    }
}
