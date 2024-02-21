using Shockky.IO;

using System.Diagnostics;

namespace Shockky.Resources;

[DebuggerDisplay("{Kind}")]
public readonly ref struct ResourceHeader
{
    public bool IsVariableLength => Kind switch
    {
        OsType.Fver or
        OsType.Fcdr or
        OsType.ABMP or
        OsType.FGEI => true,

        _ => false
    };

    public OsType Kind { get; }
    public int Length { get; }

    public ResourceHeader(OsType kind)
    {
        Kind = kind;
    }
    public ResourceHeader(ref ShockwaveReader input)
        : this((OsType)input.ReadBEInt32())
    {
        Length = IsVariableLength ?
            input.ReadVarInt() : input.ReadBEInt32();
    }

    public int GetBodySize(WriterOptions options)
    {
        int size = 0;
        size += sizeof(int);
        size += IsVariableLength ? ShockwaveWriter.GetVarIntSize(Length) : sizeof(int);
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        output.WriteBE((int)Kind);
        if (IsVariableLength)
        {
            output.WriteVarInt(Length);
        }
        else output.WriteBE(Length);
    }
}
