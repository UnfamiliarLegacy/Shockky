using System.Buffers.Binary;

using Shockky.IO;

namespace Shockky.Resources
{
    public class FileMetadata : Chunk
    {
        public CodecKind Codec { get; set; }

        public int FileLength => Header.Length;
        public bool IsBigEndian => (Kind == ResourceKind.XFIR);

        public FileMetadata()
            : base(ResourceKind.RIFX)
        { }
        public FileMetadata(ref ShockwaveReader input)
            : this(ref input, new ChunkHeader(ref input))
        {
            Header.Length = IsBigEndian ? 
                BinaryPrimitives.ReverseEndianness(Header.Length) : Header.Length;
        }
        public FileMetadata(ref ShockwaveReader input, ChunkHeader header)
            : base(header)
        {
            Codec = (CodecKind)(IsBigEndian ? 
                input.ReadInt32() : input.ReadBEInt32());
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(int);
            return size;
        }

        public override void WriteBodyTo(ShockwaveWriter output)
        {
            output.Write((int)Codec);
        }
    }
}