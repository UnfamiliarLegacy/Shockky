using Shockky.IO;

namespace Shockky.Resources
{
    public class FileInfo : Chunk
    {
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string FilePath { get; set; }

        public FileInfo()
            : base(ResourceKind.VWFI)
        { }
        public FileInfo(ref ShockwaveReader input, ChunkHeader header)
            : base(header)
        {
            input.IsBigEndian = true;

            //TODO: VList
            input.ReadBytes(input.ReadInt32());
	        int offsets = input.ReadInt16();
            input.ReadByte();
            for (short i = 0; i < offsets; i++)
            {
                input.ReadInt32();
            }
            
            input.ReadByte();
            CreatedBy = input.ReadString();
            input.ReadByte();
            ModifiedBy = input.ReadString();
            input.ReadByte();
            FilePath = input.ReadString();
        }

        public override void WriteBodyTo(ShockwaveWriter output)
        {
            throw new NotImplementedException();
        }

        public override int GetBodySize()
        {
            throw new NotImplementedException();
        }
    }
}
