using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Shockky.Lingo.Instructions;

namespace Shockky.SourceGeneration.Tests;

public sealed class InstructionGeneratorTests
{
    [Fact]
    public void OpWithoutImmediate()
    {
        string source = """
            namespace Shockky.Lingo.Instructions;

            public enum OPCode : byte
            {
                [OP] Return = 0x01,
            }
            """;
        string expected = """
            namespace Shockky.Lingo.Instructions;
            
            public sealed class ReturnIns : IInstruction
            {
                public static readonly Instruction Default = new ReturnIns();
            }
            """;

        VerifyGenerateSources(source, 
            [new InstructionGenerator()], 
            ("Return.g.cs", expected));
    }

    [Fact]
    public void OpWithImmediate()
    {
        string source = """
            namespace Shockky.Lingo.Instructions;

            public enum OPCode : byte
            {
                [OP(ImmediateKind.Integer)] PushInt = 0x41,
            }
            """;
        string expected = """
            namespace Shockky.Lingo.Instructions;
            
            public sealed class PushInt : IInstruction
            {
                public static readonly Instruction Default = new PushInt();
            }
            """;

        VerifyGenerateSources(source,
            [new InstructionGenerator()],
            ("PushInt.g.cs", expected));
    }

    /// <summary>
    /// Generates the requested sources
    /// </summary>
    /// <param name="source">The input source to process.</param>
    /// <param name="generators">The generators to apply to the input syntax tree.</param>
    /// <param name="results">The source files to compare.</param>
    private static void VerifyGenerateSources(string source, IIncrementalGenerator[] generators, params (string Filename, string? Text)[] results)
    {
        VerifyGenerateSources(source, generators, LanguageVersion.CSharp12, results);
    }

    /// <summary>
    /// Generates the requested sources
    /// </summary>
    /// <param name="source">The input source to process.</param>
    /// <param name="generators">The generators to apply to the input syntax tree.</param>
    /// <param name="languageVersion">The language version to use.</param>
    /// <param name="results">The source files to compare.</param>
    private static void VerifyGenerateSources(string source, IIncrementalGenerator[] generators, LanguageVersion languageVersion, params (string Filename, string? Text)[] results)
    {
        // Ensure Shockky is loaded
        Type observableObjectType = typeof(OPAttribute);
        
        // Get all assembly references for the loaded assemblies (easy way to pull in all necessary dependencies)
        IEnumerable<MetadataReference> references =
            from assembly in AppDomain.CurrentDomain.GetAssemblies()
            where !assembly.IsDynamic
            let reference = MetadataReference.CreateFromFile(assembly.Location)
            select reference;

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default.WithLanguageVersion(languageVersion));

        // Create a syntax tree with the input source
        CSharpCompilation compilation = CSharpCompilation.Create(
            "original",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generators)
            .WithUpdatedParseOptions((CSharpParseOptions)syntaxTree.Options);

        // Run all source generators on the input source code
        _ = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);

        // Ensure that no diagnostics were generated
        Assert.Empty(diagnostics);

        //foreach ((string filename, string? text) in results)
        //{
        //    if (text is not null)
        //    {
        //        string filePath = filename;
        //
        //        // Update the assembly version using the version from the assembly of the input generators.
        //        // This allows the tests to not need updates whenever the version of the MVVM Toolkit changes.
        //        string expectedText = text.Replace("<ASSEMBLY_VERSION>", $"\"{generators[0].GetType().Assembly.GetName().Version}\"");
        //
        //        SyntaxTree generatedTree = outputCompilation.SyntaxTrees.Single(tree => Path.GetFileName(tree.FilePath) == filePath);
        //
        //        Assert.Equal(expectedText, generatedTree.ToString());
        //    }
        //    else
        //    {
        //        // If the text is null, verify that the file was not generated at all
        //        Assert.DoesNotContain(outputCompilation.SyntaxTrees, tree => Path.GetFileName(tree.FilePath) == filename);
        //    }
        //}
        //
        //GC.KeepAlive(observableObjectType);
    }
}