namespace Shockky.Lingo.Instructions;

// WIP: For generator
[AttributeUsage(AttributeTargets.Field)]
public sealed class OPAttribute : Attribute
{
    public OPAttribute()
    { }
    public OPAttribute(ImmediateKind kind)
    { }
}

public enum ImmediateKind
{
    None = 0,
    Integer,
    Literal,
    NameIndex,
    ScriptIndex,
    Offset,
    NegativeOffset,
    ArgumentIndex,
    LocalIndex
}