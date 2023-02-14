namespace Shockky.Lingo.Instructions;

// WIP: For generator
internal class OPAttribute : Attribute
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