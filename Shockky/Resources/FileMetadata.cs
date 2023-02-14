using Shockky.IO;

namespace Shockky.Resources;

public class FileMetadata
{
    public OsType Kind { get; }
    public CodecKind Codec { get; set; }

    public int FileLength { get; }
    public bool IsBigEndian => Kind == OsType.XFIR;

    public FileMetadata(ref ShockwaveReader input)
    {
        Kind = (OsType)input.ReadBEInt32();
        FileLength = IsBigEndian ? input.ReadInt32() : input.ReadBEInt32();
        Codec = (CodecKind)(IsBigEndian ? input.ReadInt32() : input.ReadBEInt32());
    }
}
