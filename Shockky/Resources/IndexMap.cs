using Shockky.IO;

using System.Diagnostics;

namespace Shockky.Resources;

public sealed class IndexMap : IShockwaveItem, IResource
{
    public OsType Kind => OsType.imap;

    public int MemoryMapOffset { get; set; }
    public DirectorVersion Version { get; set; }

    public int Field0C { get; set; }
    public int Field10 { get; set; }
    public int Field14 { get; set; }

    public IndexMap(ref ShockwaveReader input, ReaderContext context)
    {
        int memoryMapCount = input.ReadBEInt32();
        Debug.Assert(memoryMapCount == 1);
        MemoryMapOffset = input.ReadBEInt32();
        Version = (DirectorVersion)input.ReadBEInt32();
        Field0C = input.ReadBEInt32();
        Field10 = input.ReadBEInt32();
        Field14 = input.ReadBEInt32();
    }

    public int GetBodySize(WriterOptions options)
    {
        int size = 0;
        size += sizeof(int);
        size += sizeof(int);
        size += sizeof(int);
        size += sizeof(int);
        size += sizeof(int);
        size += sizeof(int);
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        output.WriteBE(1);
        output.WriteBE(MemoryMapOffset);
        output.WriteBE((int)Version);
        output.WriteBE(Field0C);
        output.WriteBE(Field10);
        output.WriteBE(Field14);
    }
}
