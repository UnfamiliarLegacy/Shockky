using Shockky.IO;

namespace Shockky.Resources;

public sealed class FileCompressionTypes : IShockwaveItem, IResource
{
    public OsType Kind => OsType.Fcdr;

    public short CompressionTypeId { get; set; }
    public int ImageQuality { get; set; }
    public short ImageTypes { get; set; }
    public short DirTypes { get; set; }
    public int CompressionLevel { get; set; }
    public int Speed { get; set; }
    public string Name { get; set; }

    public FileCompressionTypes()
    { }
    public FileCompressionTypes(ref ShockwaveReader input, ReaderContext context)
    {
        using ZLibShockwaveReader decompressedInput = IResource.CreateDeflateReader(ref input);

        CompressionTypeId = decompressedInput.ReadInt16();
        ImageQuality = decompressedInput.ReadInt32();
        ImageTypes = decompressedInput.ReadInt16();
        DirTypes = decompressedInput.ReadInt16();
        CompressionLevel = decompressedInput.ReadInt32();
        Speed = decompressedInput.ReadInt32();
        
        if (CompressionTypeId == 256)
            Name = decompressedInput.ReadCString();
    }

    public int GetBodySize(WriterOptions options) => throw new NotImplementedException();
    public void WriteTo(ShockwaveWriter output, WriterOptions options) => throw new NotImplementedException();
}
