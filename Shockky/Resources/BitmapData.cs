using Shockky.IO;
using Shockky.Resources.Cast;

namespace Shockky.Resources
{
    public class BitmapData : BinaryData, ICastMemberMediaChunk<BitmapCastProperties>
    {
        public byte BitDepth { get; set; }
        public BitmapFlags Flags { get; set; }

        public int Stride { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public BitmapData()
            : base(ResourceKind.BITD)
        { }
        public BitmapData(ref ShockwaveReader input, ChunkHeader header)
            : base(ref input, header)
        { }

        public void PopulateMedia(BitmapCastProperties properties)
        {
            BitDepth = properties.BitDepth;

            Flags = properties.Flags;

            Stride = properties.Stride;
            Width = properties.Rectangle.Width;
            Height = properties.Rectangle.Height;

            int outputLength = Stride * Height;
            if (outputLength == 0 || Data.Length == outputLength)
                return;

            Span<byte> outputSpan = outputLength < 1024 ? stackalloc byte[outputLength] : new byte[outputLength];
            var output = new ShockwaveWriter(outputSpan, false);
            var input = new ShockwaveReader(Data.AsSpan());
            
            while (input.IsDataAvailable)
            {
                byte marker = input.ReadByte();
                if ((marker & 0x80) != 0)
                {
                    int length = 257 - marker;
                    output.CurrentSpan
                        .Slice(0, length)
                        .Fill(input.ReadByte());
                    output.Advance(length);
                }
                else output.Write(input.ReadBytes(marker + 1));
            }

            Data = outputSpan.ToArray(); //TODO: Can I avoid this copy? I don't think so
        }
    }
}
