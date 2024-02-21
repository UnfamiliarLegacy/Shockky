using Shockky.IO;

namespace Shockky.Resources.Cast;

public class ScriptCastProperties : IMemberProperties
{
    public ScriptKind Kind { get; set; }

    public ScriptCastProperties()
    { }
    public ScriptCastProperties(ref ShockwaveReader input, ReaderContext context)
    {
        Kind = (ScriptKind)input.ReadInt16();
    }

    public int GetBodySize(WriterOptions options)
    {
        int size = 0;
        size += sizeof(short);
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        output.Write((short)Kind);
    }
}
