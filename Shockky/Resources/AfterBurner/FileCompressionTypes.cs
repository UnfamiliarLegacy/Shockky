using Shockky.IO;

namespace Shockky.Resources
{
    public class FileCompressionTypes : Chunk
    {
        public short CompressionTypeId { get; set; }
        public int ImageQuality { get; set; }
        public short ImageTypes { get; set; }
        public short DirTypes { get; set; }
        public int CompressionLevel { get; set; }
        public int Speed { get; set; }
        public string Name { get; set; }

        public FileCompressionTypes()
            : base(ResourceKind.Fcdr)
        { }
        public FileCompressionTypes(ref ShockwaveReader input, ChunkHeader header)
            : base(header)
        {
            using DeflateShockwaveReader decompressedInput = CreateDeflateReader(ref input);

            CompressionTypeId = decompressedInput.ReadInt16();
            ImageQuality = decompressedInput.ReadInt32();
            ImageTypes = decompressedInput.ReadInt16();
            DirTypes = decompressedInput.ReadInt16();
            CompressionLevel = decompressedInput.ReadInt32();
            Speed = decompressedInput.ReadInt32();
            
            if (CompressionTypeId == 256)
                Name = decompressedInput.ReadCString();
        }

        public override int GetBodySize()
        {
            throw new NotImplementedException();
        }

        public override void WriteBodyTo(ShockwaveWriter output)
        {
            throw new NotImplementedException(nameof(FileCompressionTypes));
            //TODO: Compressor
            output.Write(CompressionTypeId);
            output.Write(ImageQuality);
            output.Write(ImageTypes);
            output.Write(DirTypes);
            output.Write(CompressionLevel);
            output.Write(Speed);

            if (CompressionTypeId == 256)
                output.WriteCString(Name);
        }
    }
}
