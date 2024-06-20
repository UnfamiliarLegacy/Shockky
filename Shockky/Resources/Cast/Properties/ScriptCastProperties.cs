using Shockky.IO;
using Shockky.Resources.Enum;

namespace Shockky.Resources.Cast.Properties;

public class ScriptCastProperties : IMemberProperties
{
    public ScriptKind Kind { get; set; }

    public ScriptCastProperties()
    { }
    public ScriptCastProperties(ref ShockwaveReader input, ReaderContext context)
    {
        Kind = (ScriptKind)input.ReadInt16BigEndian();
    }

    public int GetBodySize(WriterOptions options)
    {
        int size = 0;
        size += sizeof(short);
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        output.WriteInt16BigEndian((short)Kind);
    }
}
