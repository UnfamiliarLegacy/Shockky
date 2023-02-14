using System;

namespace Shockky.Generators.Models;

internal sealed record InstructionInfo(string OpName, uint OpValue, ImmediateKind ImmediateKind)
{
    public static InstructionInfo From() => throw new NotImplementedException();
}

internal enum ImmediateKind
{
    None = 0,
    Integer,
    Literal,
    NameIndex,
    ScriptIndex,
    Offset,
    NegativeOffset,
    ArgumentIndex,
    LocalVarIndex
}
