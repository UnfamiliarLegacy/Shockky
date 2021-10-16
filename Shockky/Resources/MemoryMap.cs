using Shockky.IO;

namespace Shockky.Resources
{
    public class MemoryMap : Chunk
    {
        public const short ENTRIES_OFFSET = 24;
        public const short ENTRY_SIZE = 20;

        public ChunkEntry[] Entries { get; set; }

        public int LastJunkIndex { get; set; }
        public int SomeLinkedIndex { get; set; }
        public int LastFreeIndex { get; set; }

        public ChunkEntry this[int index] => Entries[index];

        public MemoryMap()
            : base(ResourceKind.mmap)
        { }
        public MemoryMap(ref ShockwaveReader input, ChunkHeader header)
            : base(header)
        {
            input.ReadBEInt16();
            input.ReadBEInt16();

            input.ReadBEInt32();
            Entries = new ChunkEntry[input.ReadBEInt32()];

            LastJunkIndex = input.ReadBEInt32();
            SomeLinkedIndex = input.ReadBEInt32();
            LastFreeIndex = input.ReadBEInt32();

            System.Diagnostics.Debug.Assert(SomeLinkedIndex == -1); //TODO:
            for (int i = 0; i < Entries.Length; i++)
            {
                Entries[i] = new ChunkEntry(ref input);
            }
        }
        
        public override void WriteBodyTo(ShockwaveWriter output)
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
                entry.WriteTo(output);
            }
        }

        public override int GetBodySize()
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
    }
}
