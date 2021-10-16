using System.Diagnostics;

using Shockky.IO;

namespace Shockky.Resources
{
    [DebuggerDisplay("[{Kind}] Length: {Header.Length}")]
    public abstract class Chunk : ShockwaveItem
    {
        public ChunkHeader Header { get; set; }
        public Queue<object> Remnants { get; set; }

        public ResourceKind Kind => Header.Kind;

        protected Chunk(ResourceKind kind)
            : this(new ChunkHeader(kind))
        { }
        protected Chunk(ChunkHeader header)
        {
            Header = header;

            Remnants = new Queue<object>();
        }

        public DeflateShockwaveReader CreateDeflateReader(ref ShockwaveReader input)
        {
            input.Advance(2); //Skip ZLib header

            int dataLeft = Header.Length - input.Position;
            byte[] compressedData = input.ReadBytes(dataLeft).ToArray();

            return new DeflateShockwaveReader(compressedData, input.IsBigEndian);
        }

        public override void WriteTo(ShockwaveWriter output)
        {
            Header.Length = GetBodySize();
            Header.WriteTo(output);

            WriteBodyTo(output);
        }
        public abstract void WriteBodyTo(ShockwaveWriter output);

        public static Chunk Read(ref ShockwaveReader input)
        {
            return Read(ref input, new ChunkHeader(ref input));
        }
        public static Chunk Read(ref ShockwaveReader input, AfterBurnerMapEntry entry)
        {
            return entry.IsCompressed ? input.ReadCompressedChunk(entry) : 
                Read(ref input, entry.Header);
        }
        public static Chunk Read(ref ShockwaveReader input, ChunkHeader header)
        {
            ReadOnlySpan<byte> chunkSpan = input.ReadBytes(header.Length);
            var chunkInput = new ShockwaveReader(chunkSpan, input.IsBigEndian);

            return header.Kind switch
            {
                ResourceKind.RIFX => new FileMetadata(ref chunkInput, header),
                
                ResourceKind.Fver => new FileVersion(ref chunkInput, header),
                ResourceKind.Fcdr => new FileCompressionTypes(ref chunkInput, header),
                ResourceKind.ABMP => new AfterburnerMap(ref chunkInput, header),
                ResourceKind.FGEI => new FileGzipEmbeddedImage(header),
                
                ResourceKind.imap => new InitialMap(ref chunkInput, header),
                ResourceKind.mmap => new MemoryMap(ref chunkInput, header),
                ResourceKind.KEYPtr => new AssociationTable(ref chunkInput, header),

                ResourceKind.VWCF => new Config(ref chunkInput, header),
                ResourceKind.DRCF => new Config(ref chunkInput, header),

                ResourceKind.VWLB => new ScoreLabels(ref chunkInput, header),
                ResourceKind.VWFI => new FileInfo(ref chunkInput, header),
                
                ResourceKind.Lnam => new LingoNames(ref chunkInput, header),
                ResourceKind.Lscr => new LingoScript(ref chunkInput, header),
                ResourceKind.Lctx or ResourceKind.LctX => new LingoContext(ref chunkInput, header),
                
                ResourceKind.CASPtr => new CastAssociationTable(ref chunkInput, header),
                ResourceKind.CASt => new CastMemberProperties(ref chunkInput, header),
                
                ResourceKind.SCRF => new ScoreReference(ref chunkInput, header),
                ResourceKind.Sord => new ScoreOrder(ref chunkInput, header),
                ResourceKind.CLUT => new Palette(ref chunkInput, header),
                ResourceKind.STXT => new StyledText(ref chunkInput, header),
                
                ResourceKind.snd => new SoundData(ref chunkInput, header),

                ResourceKind.Fmap => new FontMap(ref chunkInput, header),
                
                ResourceKind.GRID => new Grid(ref chunkInput, header),
                ResourceKind.FCOL => new FavoriteColors(ref chunkInput, header),
                
                ResourceKind.FXmp => new FontMap(ref chunkInput, header),
                ResourceKind.BITD => new BitmapData(ref chunkInput, header),

                _ => new UnknownChunk(ref chunkInput, header),
            };
        }
    }
}
