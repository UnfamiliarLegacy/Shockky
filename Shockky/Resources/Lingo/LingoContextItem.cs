using Shockky.IO;

namespace Shockky.Resources;

public class LingoContextItem : IShockwaveItem
{
    public int ChunkIndex { get; set; }
    public LingoContextItemFlags Flags { get; set; }

    /// <summary>
    /// Points to next free context item.
    /// </summary>
    public short Link { get; set; }

    public LingoContextItem()
    { }
    public LingoContextItem(ref ShockwaveReader input)
    {
        input.ReadInt32();
        ChunkIndex = input.ReadInt32();
        Flags = (LingoContextItemFlags)input.ReadInt16();
        Link = input.ReadInt16();
    }

    public int GetBodySize(WriterOptions options)
    {
        int size = 0;
        size += sizeof(int);
        size += sizeof(int);
        size += sizeof(short);
        size += sizeof(short);
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        output.Write(0);
        output.Write(ChunkIndex);
        output.Write((short)Flags);
        output.Write(Link);
    }
}
