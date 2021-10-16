using Shockky.IO;

namespace Shockky.Resources
{
    public class Score : Chunk
    {
        public Score()
            : base(ResourceKind.VWSC)
        { }
        public Score(ref ShockwaveReader input, ChunkHeader header)
            : base(header)
        {
            input.IsBigEndian = true;

            int totalLength = input.ReadInt32();
            int headerType = input.ReadInt32(); //-3

            throw new NotImplementedException();
        }

        public override int GetBodySize()
        {
            throw new NotImplementedException();
            int size = 0;
            size += sizeof(int);
            size += sizeof(int);

            size += sizeof(int);
            size += sizeof(int);
            
            size += sizeof(int);
            size += sizeof(int);
            return size;
        }

        public override void WriteBodyTo(ShockwaveWriter output)
        {
            throw new NotImplementedException();
        }
    }
}
