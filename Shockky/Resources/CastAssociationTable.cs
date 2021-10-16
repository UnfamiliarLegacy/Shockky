using Shockky.IO;

namespace Shockky.Resources
{
    public class CastAssociationTable : Chunk
    {
        public int[] Members { get; set; }

        public CastAssociationTable()
            : base(ResourceKind.CASPtr)
        { }
        public CastAssociationTable(ref ShockwaveReader input, ChunkHeader header)
            : base(header)
        {
            input.IsBigEndian = true;

            Members = new int[header.Length / sizeof(int)];
            for (int i = 0; i < Members.Length; i++)
            {
                Members[i] = input.ReadInt32();
            }
        }

        public override void WriteBodyTo(ShockwaveWriter output)
        {
            for (int i = 0; i < Members.Length; i++)
            {
                output.Write(Members[i]);
            }
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += Members.Length * sizeof(int);
            return size;
        }
    }
}
