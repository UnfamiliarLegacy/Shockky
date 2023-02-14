using Shockky.IO;

namespace Shockky.Resources;

public sealed class MemoryMap : IShockwaveItem, IResource
{
    public OsType Kind => OsType.mmap;

    public const short ENTRIES_OFFSET = 24;
    public const short ENTRY_SIZE = 20;

    public ResourceEntry[] Entries { get; set; }

    public int LastJunkIndex { get; set; }
    public int SomeLinkedIndex { get; set; }
    public int LastFreeIndex { get; set; }

    public ResourceEntry this[int index] => Entries[index];

    public MemoryMap(ref ShockwaveReader input, ReaderContext context)
    {
        input.ReadBEInt16();
        input.ReadBEInt16();

        input.ReadBEInt32();
        var entries = new ResourceEntry[input.ReadBEInt32()];

        int lastJunkIndex = input.ReadBEInt32();
        int someLinkedIndex = input.ReadBEInt32();
        int lastFreeIndex = input.ReadBEInt32();

        for (int i = 0; i < Entries.Length; i++)
        {
            Entries[i] = new ResourceEntry(ref input, context);
        }
    }

    public int GetBodySize(WriterOptions options)
    {
        int size = 0;
        size += sizeof(short);
        size += sizeof(short);
        size += sizeof(int);
        size += sizeof(int);
        size += sizeof(int);
        size += sizeof(int);
        size += sizeof(int);
        size += Entries.Length * ENTRY_SIZE;
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        output.WriteBE(ENTRIES_OFFSET);
        output.WriteBE(ENTRY_SIZE);

        output.WriteBE(Entries.Length);
        output.WriteBE(Entries.Length);

        output.WriteBE(LastJunkIndex);
        output.WriteBE(SomeLinkedIndex);
        output.WriteBE(LastFreeIndex);
        foreach (var entry in Entries)
        {
            entry.WriteTo(output, options);
        }
    }
}
