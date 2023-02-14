using System.Diagnostics;

using Shockky.IO;

namespace Shockky.Resources;

[DebuggerDisplay("[{Index}] {Kind} Offset: {Offset}")]
public sealed class AfterburnerMapEntry : IShockwaveItem
{
    public OsType Kind { get; set; }
    public int Index { get; set; }
    public int Offset { get; set; }
    public int Length { get; set; }
    public int DecompressedLength { get; set; }
    public int CompressionType { get; set; }

    public bool IsCompressed => CompressionType == 0;

    public AfterburnerMapEntry(ZLibShockwaveReader input)
    {
        Index = input.ReadVarInt();
        Offset = input.ReadVarInt();
        Length = input.ReadVarInt();
        DecompressedLength = input.ReadVarInt();
        CompressionType = input.ReadVarInt();
        Kind = (OsType)input.ReadBEInt32();
    }

    public int GetBodySize(WriterOptions options)
    {
        int size = 0;
        size += ShockwaveWriter.GetVarIntSize(Index);
        size += ShockwaveWriter.GetVarIntSize(Offset);
        size += ShockwaveWriter.GetVarIntSize(Length);
        size += ShockwaveWriter.GetVarIntSize(DecompressedLength);
        size += ShockwaveWriter.GetVarIntSize(CompressionType);
        size += sizeof(int);
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        output.WriteVarInt(Index);
        output.WriteVarInt(Offset);
        output.WriteVarInt(Length);
        output.WriteVarInt(DecompressedLength);
        output.WriteVarInt(CompressionType);
        output.WriteBE((int)Kind);
    }

    public static AfterburnerMapEntry Read(ref ShockwaveReader input, ReaderContext context)
    {
        throw new NotImplementedException();
    }
}
