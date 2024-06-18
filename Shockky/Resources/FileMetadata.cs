using Shockky.IO;

namespace Shockky.Resources;

public class FileMetadata
{
    public OsType Kind { get; }
    public CodecKind Codec { get; set; }

    public int FileLength { get; }
    public bool IsLittleEndian => Kind == OsType.XFIR;

    public FileMetadata(ref ShockwaveReader input)
    {
        Kind = (OsType)input.ReadInt32BigEndian();
        FileLength = IsLittleEndian ? input.ReadInt32LittleEndian() : input.ReadInt32BigEndian();
        Codec = (CodecKind)(IsLittleEndian ? input.ReadInt32LittleEndian() : input.ReadInt32BigEndian());
    }
}
