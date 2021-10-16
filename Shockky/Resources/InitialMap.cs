using Shockky.IO;

namespace Shockky.Resources
{
    public class InitialMap : Chunk
    {
        public int[] MemoryMapOffsets { get; set; }

        public DirectorVersion Version { get; set; }

        public InitialMap()
            : base(ResourceKind.imap)
        { }
        public InitialMap(ref ShockwaveReader input, ChunkHeader header)
            : base(header)
        {
            MemoryMapOffsets = new int[input.ReadBEInt32()];
            for (int i = 0; i < MemoryMapOffsets.Length; i++)
            {
                MemoryMapOffsets[i] = input.ReadBEInt32();
            }
            Version = (DirectorVersion)input.ReadBEInt32();
            input.ReadBEInt16();
            input.ReadBEInt16();
            input.ReadBEInt32();
            Remnants.Enqueue(input.ReadBEInt32());
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(int);
            size += sizeof(int) * MemoryMapOffsets.Length;
            size += sizeof(int);
            size += sizeof(short);
            size += sizeof(short);
            size += sizeof(int);
            size += sizeof(int);
            return size;
        }

        public override void WriteBodyTo(ShockwaveWriter output)
        {
            output.WriteBE(MemoryMapOffsets.Length);
            for (int i = 0; i < MemoryMapOffsets.Length; i++)
            {
                output.WriteBE(MemoryMapOffsets[i]);
            }
            output.WriteBE((int)Version);
            output.WriteBE((short)0);
            output.WriteBE((short)0);
            output.WriteBE(0);
            output.WriteBE((int)Remnants.Dequeue());
        }
    }
}
