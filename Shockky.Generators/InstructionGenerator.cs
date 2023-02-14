using System;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shockky.Generators;

// WIP: Instruction generator, heavily inspired by csnover's eq-rs macros

[Generator(LanguageNames.CSharp)]
public sealed class InstructionGenerator : IIncrementalGenerator
{
    private const string OPAttributeFullName = "Shockky.Lingo.Instructions.OPAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Get all declared class symbols with the [OP] attribute.
        IncrementalValuesProvider<SyntaxNode> enumMemberDeclarations = context.SyntaxProvider
            .ForAttributeWithMetadataName(OPAttributeFullName,
                predicate: static (node, token) => node is EnumMemberDeclarationSyntax,
                transform: static (context, token) => 
                {
                    // TODO: Collect InstructionInfo

                    return context.TargetNode;
                });

        // Filter

        // context.RegisterSourceOutput(..);
    }

    public void GetTarget(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken) => throw new NotImplementedException();
}
