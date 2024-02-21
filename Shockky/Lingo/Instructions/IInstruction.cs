namespace Shockky.Lingo.Instructions;

/// <summary>
/// Represents an lingo instruction with an optional immediate value.
/// </summary>
public partial interface IInstruction
{
    OPCode OP { get; }
    int Immediate { get; set; }
}