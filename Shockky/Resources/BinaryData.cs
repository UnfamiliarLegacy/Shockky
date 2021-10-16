using Shockky.IO;

namespace Shockky.Resources
{
    public abstract class BinaryData : Chunk
    {
        public byte[] Data { get; set; }

        protected BinaryData(ResourceKind kind)
            : base(kind)
        { }
        protected BinaryData(ref ShockwaveReader input, ChunkHeader header) 
            : base(header)
        {
            Data = input.ReadBytes(header.Length).ToArray();
        }

        public override int GetBodySize() => Data.Length;
        public override void WriteBodyTo(ShockwaveWriter output)
        {
            output.Write(Data);
        }
    }
}
