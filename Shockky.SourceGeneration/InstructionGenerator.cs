using System;
using System.Threading;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Shockky.SourceGeneration.Models;
using Shockky.SourceGeneration.Helpers;
using Shockky.SourceGeneration.Extensions;
using Shockky.SourceGeneration.Diagnostics;

namespace Shockky.SourceGeneration;

[Generator(LanguageNames.CSharp)]
public sealed class InstructionGenerator : IIncrementalGenerator
{
    private const string OPAttributeFullName = "Shockky.Lingo.Instructions.OPAttribute";
    private const string InstructionNamespace = "Shockky.Lingo.Instructions";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Gather info for all the annotated instruction definitions
        IncrementalValuesProvider<Result<InstructionInfo?>> instructionInfoResults =
            context.SyntaxProvider.ForAttributeWithMetadataName(OPAttributeFullName,
                predicate: static (node, token) => node is EnumMemberDeclarationSyntax,
                transform: static (context, token) =>
                {
                    var enumMemberDeclaration = (EnumMemberDeclarationSyntax)context.TargetNode;
                    var fieldSymbol = (IFieldSymbol)context.TargetSymbol;

                    _ = TryGetInfo(
                        enumMemberDeclaration,
                        fieldSymbol,
                        context.SemanticModel,
                        token,
                        out InstructionInfo? instructionInfo,
                        out ImmutableArray<DiagnosticInfo> diagnostics);

                    return new Result<InstructionInfo?>(instructionInfo, diagnostics);
                })
            .WithTrackingName(nameof(instructionInfoResults));

        context.ReportDiagnostics(instructionInfoResults.Select(static (item, _) => item.Errors));

        // Drop the nulls
        IncrementalValuesProvider<InstructionInfo> instructions = instructionInfoResults
            .Where(static (result) => result.Value is not null)
            .Select(static (result, token) => result.Value!);

        // Generate instruction definitions
        context.RegisterSourceOutput(instructions, static (context, instructionInfo) =>
        {
            using IndentedTextWriter writer = new();

            writer.WriteLine($"namespace {InstructionNamespace};");
            writer.WriteLine();

            WriteInstructionSyntax(writer, instructionInfo);

            context.AddSource($"{InstructionNamespace}.{instructionInfo.Name}.g.cs", writer.ToString());
        });

        // Gather all instructions and generate factory method
        var allInstructions = instructions.Collect();

        context.RegisterSourceOutput(allInstructions, static (context, item) =>
        {
            using IndentedTextWriter writer = new();

            writer.WriteLine($"namespace {InstructionNamespace};");
            writer.WriteLine();
            
            using (writer.WriteBlock("public partial interface IInstruction"))
            {
                writer.WriteGeneratedAttributes(nameof(InstructionGenerator));
                using (writer.WriteBlock("public static IInstruction Read(ref global::Shockky.IO.ShockwaveReader input)"))
                {
                    writer.WriteLine("""
                        byte op = input.ReadByte();
                        int immediate = op >> 6 switch 
                        {
                            1 => input.ReadByte(),
                            2 => input.ReadInt16(),
                            3 => input.ReadInt32(),
                            _ => 0
                        };
                        """, true);

                    writer.WriteLine();
                    using (writer.WriteBlock("return (OPCode)op switch", isExpression: true))
                    {
                        foreach (var instruction in item.AsSpan())
                        {
                            writer.WriteLine($"OPCode.{instruction.Name} => new {instruction.Name}(),");
                        }
                        writer.WriteLine("_ => throw null");
                    }
                }
            }

            context.AddSource($"{InstructionNamespace}.IInstruction.Read.g.cs", writer.ToString());
        });
    }

    internal static bool TryGetInfo(
        EnumMemberDeclarationSyntax enumMemberSyntax,
        IFieldSymbol fieldSymbol,
        SemanticModel semanticModel,
        CancellationToken cancellationToken,
        [NotNullWhen(true)] out InstructionInfo? instructionInfo,
        out ImmutableArray<DiagnosticInfo> diagnostics)
    {
        // TODO: More sanity checks
        using ImmutableArrayBuilder<DiagnosticInfo> diagnosticBuilder = ImmutableArrayBuilder<DiagnosticInfo>.Rent();

        // Get ImmediateKind from the attribute if given.
        ImmediateKind immediateKind = ImmediateKind.None;

        if (fieldSymbol.TryGetAttributeWithFullyQualifiedMetadataName(OPAttributeFullName, out AttributeData? attributeData) &&
            attributeData.ConstructorArguments is [{ Value: int immediateKindValue }])
        {
            immediateKind = (ImmediateKind)immediateKindValue;
        }

        // Get instruction OP value from the assigned constant expression
        if (enumMemberSyntax.EqualsValue is null)
        {
            diagnosticBuilder.Add(
                DiagnosticDescriptors.MissingExplicitValueOnOPAttributeAnnotatedEnumMember,
                fieldSymbol,
                fieldSymbol.ContainingType, fieldSymbol.Name);

            instructionInfo = null;
            diagnostics = diagnosticBuilder.ToImmutable();

            return false;
        }

        var equalsValueExpression = enumMemberSyntax.EqualsValue.Value;
        if (semanticModel.GetOperation(equalsValueExpression) is not IOperation equalsValueOperation)
        {
            throw new ArgumentException("Failed to retrieve an operation for the equals expression");
        }

        var enumValueConstant = TypedConstantInfo.From(
            equalsValueOperation, semanticModel, equalsValueExpression, cancellationToken);

        instructionInfo = new InstructionInfo(fieldSymbol.Name, enumValueConstant, immediateKind);
        diagnostics = diagnosticBuilder.ToImmutable();

        return true;
    }

    internal static void WriteInstructionSyntax(IndentedTextWriter writer,
        InstructionInfo instruction)
    {
        writer.WriteGeneratedAttributes(nameof(InstructionGenerator));

        using (writer.WriteBlock($"public sealed class {instruction.Name} : IInstruction"))
        {
            bool hasImmediate = instruction.ImmediateKind is not ImmediateKind.None;

            // Offer cached instance for instruction with no immediate
            writer.WriteLineIf(!hasImmediate, $"public static readonly {instruction.Name} Default = new();");
            writer.WriteLineIf(!hasImmediate);

            writer.WriteLine($"public OPCode OP => OPCode.{instruction.Name};");
            writer.WriteLine();

            // If the OP has no immediate, hide the property representing it with explicit implementation.
            if (!hasImmediate)
            {
                writer.WriteLine("int IInstruction.Immediate { get; set; }");
            }
            else writer.WriteLine("public int Immediate { get; set; }");
            writer.WriteLine();

            if (hasImmediate)
            {
                writer.WriteLine("""
                    public int GetSize()
                    {
                        uint imm = (uint)Immediate;
                        if (imm <= byte.MaxValue) return sizeof(OPCode) + 1;
                        else if (imm <= ushort.MaxValue) return sizeof(OPCode) + 2;
                        else return sizeof(OPCode) + 4;
                    }
                    """, true);
            }
            else writer.WriteLine("public int GetSize() => sizeof(OPCode);");

            writer.WriteLine();
            using (writer.WriteBlock("public void WriteTo(global::Shockky.IO.ShockwaveWriter output)"))
            {
                if (hasImmediate)
                {
                    writer.WriteLine("""
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
                        """, true);
                }
                else writer.WriteLine("output.Write((byte)OP);");
            }
        }
    }
}
