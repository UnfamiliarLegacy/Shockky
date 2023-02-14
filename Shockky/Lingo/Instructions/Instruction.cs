using Shockky.IO;

namespace Shockky.Lingo.Instructions;

// WIP: Prototyping instruction generator.

/// <summary>
/// Represents an lingo instruction with an immediate value.
/// </summary>
public interface IImmediateInstruction
{
    int Value { get; set; }
}

public interface IPrimitive
{ }

public abstract class Instruction
{
    public abstract OPCode OP { get; }
    public abstract int Immediate { get; set; }
}