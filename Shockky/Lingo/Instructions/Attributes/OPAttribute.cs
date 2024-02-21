namespace Shockky.Lingo.Instructions;

[AttributeUsage(AttributeTargets.Field)]
public sealed class OPAttribute : Attribute
{
    public OPAttribute()
    { }
    public OPAttribute(ImmediateKind kind)
    { }
}