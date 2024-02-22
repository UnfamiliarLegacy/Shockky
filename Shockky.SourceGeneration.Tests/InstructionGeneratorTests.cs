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
        string expectedSyntaxForReturn = """
            namespace Shockky.Lingo.Instructions;
            
            [global::System.CodeDom.Compiler.GeneratedCode("InstructionGenerator", <ASSEMBLY_VERSION>)]
            [global::System.Diagnostics.DebuggerNonUserCode]
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            public sealed class Return : IInstruction
            {
                public static readonly Return Default = new();

                /// <inheritdoc />
                public OPCode OP => OPCode.Return;

                [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                [global::System.Diagnostics.DebuggerBrowsable(global::System.Diagnostics.DebuggerBrowsableState.Never)]
                public int Immediate { get; set; }

                public int GetSize() => sizeof(OPCode);

                public void WriteTo(global::Shockky.IO.ShockwaveWriter output)
                {
                    output.Write((byte)OP);
                }
            }
            """;

        VerifyGenerateSources(source,
            [new InstructionGenerator()],
            ("Shockky.Lingo.Instructions.Return.g.cs", expectedSyntaxForReturn));
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
            
            [global::System.CodeDom.Compiler.GeneratedCode("InstructionGenerator", <ASSEMBLY_VERSION>)]
            [global::System.Diagnostics.DebuggerNonUserCode]
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            public sealed class PushInt : IInstruction
            {
                /// <inheritdoc />
                public OPCode OP => OPCode.PushInt;

                public int Immediate { get; set; }

                public int GetSize()
                {
                    uint imm = (uint)Immediate;
                    if (imm <= byte.MaxValue) return sizeof(OPCode) + 1;
                    else if (imm <= ushort.MaxValue) return sizeof(OPCode) + 2;
                    else return sizeof(OPCode) + 4;
                }

                public void WriteTo(global::Shockky.IO.ShockwaveWriter output)
                {
                    byte op = (byte)OP;

                    if (Immediate <= byte.MaxValue)
                    {
                        output.Write(op);
                        output.Write((byte)Immediate);
                    }
                    else if (Immediate <= ushort.MaxValue)
                    {
                        output.Write(op + 0x40);
                        output.Write((ushort)Immediate);
                    }
                    else
                    {
                        output.Write(op + 0x80);
                        output.Write(Immediate);
                    }
                }
            }
            """;

        VerifyGenerateSources(source,
            [new InstructionGenerator()],
            ("Shockky.Lingo.Instructions.PushInt.g.cs", expected));
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

        foreach ((string filename, string? text) in results)
        {
            if (text is not null)
            {
                string filePath = filename;

                // Update the assembly version using the version from the assembly of the input generators.
                // This allows the tests to not need updates whenever the version of the MVVM Toolkit changes.
                string expectedText = text.Replace("<ASSEMBLY_VERSION>", $"\"{generators[0].GetType().Assembly.GetName().Version}\"");

                SyntaxTree generatedTree = outputCompilation.SyntaxTrees.Single(tree => Path.GetFileName(tree.FilePath) == filePath);

                Assert.Equal(expectedText, generatedTree.ToString());
            }
            else
            {
                // If the text is null, verify that the file was not generated at all
                Assert.DoesNotContain(outputCompilation.SyntaxTrees, tree => Path.GetFileName(tree.FilePath) == filename);
            }
        }

        GC.KeepAlive(observableObjectType);
    }
}