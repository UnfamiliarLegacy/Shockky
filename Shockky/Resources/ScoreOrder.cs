using Shockky.IO;
using Shockky.Resources.Cast;

namespace Shockky.Resources
{
    /// <summary>
    /// Represents list of all cast members in the movie, sorted by the order they appear in the Score.
    /// </summary>
    public class ScoreOrder : Chunk
    {
        public CastMemberId[] Entries { get; set; }

        public ScoreOrder()
            : base(ResourceKind.Sord)
        { }
        public ScoreOrder(ref ShockwaveReader input, ChunkHeader header)
            : base(header)
        {
            input.IsBigEndian = true;

            input.ReadInt32();
            input.ReadInt32();

            Entries = new CastMemberId[input.ReadInt32()];
            input.ReadInt32();
            
            input.ReadInt16();
            input.ReadInt16(); //TODO: dir <= 0x500 ? sizeof(short) : sizeof(short) * 2.

            for (int i = 0; i < Entries.Length; i++)
            {
                Entries[i] = new(input.ReadInt16(), input.ReadInt16());
            }
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(int);
            size += sizeof(int);

            size += sizeof(int);

            size += sizeof(short);
            size += sizeof(short);

            size += sizeof(short) * 2 * Entries.Length;
            return size;
        }

        public override void WriteBodyTo(ShockwaveWriter output)
        {
            const short ENTRIES_OFFSET = 20;
            const short ENTRY_SIZE = sizeof(short) + sizeof(short);

            output.Write(0);
            output.Write(0);

            output.Write(Entries.Length);
            output.Write(Entries.Length);

            output.Write(ENTRIES_OFFSET);
            output.Write(ENTRY_SIZE);

            foreach (CastMemberId memberId in Entries)
            {
                output.Write(memberId);
            }
        }
    }
}
