using Shockky.IO;

namespace Shockky.Resources
{
    public class ScoreReference : Chunk
    {
        public Dictionary<short, int> Entries { get; }

        public ScoreReference()
            : base(ResourceKind.SCRF)
        { }
        public ScoreReference(ref ShockwaveReader input, ChunkHeader header) 
            : base(header)
        {
            input.ReadInt32();
            input.ReadInt32();

            int entryCount = input.ReadInt32();
            input.ReadInt32();

            input.ReadInt16();
            input.ReadInt16();

            Remnants.Enqueue(input.ReadInt32());

            Entries = new Dictionary<short, int>(entryCount);
            for (int i = 0; i < entryCount; i++)
            {
                Entries.Add(input.ReadInt16(), input.ReadInt32());
            }
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(int);
            size += sizeof(int);
            size += sizeof(int);
            size += sizeof(int);
            size += sizeof(short);
            size += sizeof(short);
            size += sizeof(int);
            size += Entries.Count * (sizeof(short) + sizeof(int));
            return size;
        }

        public override void WriteBodyTo(ShockwaveWriter output)
        {
            const short ENTRY_OFFSET = 24;
            const short UNKNOWN = 8;
            
            output.Write(0);
            output.Write(0);

            output.Write(Entries.Count);
            output.Write(Entries.Count);

            output.Write(ENTRY_OFFSET);
            output.Write(UNKNOWN);

            output.Write((int)Remnants.Dequeue());
            foreach ((short number, int castMapPtrId) in Entries)
            {
                output.Write(number);
                output.Write(castMapPtrId);
            }
        }
    }
}
