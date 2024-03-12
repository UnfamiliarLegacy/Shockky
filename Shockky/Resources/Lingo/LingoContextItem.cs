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
        input.ReadInt32LittleEndian();
        ChunkIndex = input.ReadInt32LittleEndian();
        Flags = (LingoContextItemFlags)input.ReadInt16LittleEndian();
        Link = input.ReadInt16LittleEndian();
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
        output.WriteInt32LittleEndian(0);
        output.WriteInt32LittleEndian(ChunkIndex);
        output.WriteInt16LittleEndian((short)Flags);
        output.WriteInt16LittleEndian(Link);
    }
}
