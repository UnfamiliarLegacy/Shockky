using Shockky.IO;

namespace Shockky.Resources;

public sealed class FileVersion : IResource, IShockwaveItem
{
    public OsType Kind => OsType.Fver;

    public DirectorVersion Version { get; set; }
    public string VersionString { get; set; }

    public FileVersion(ref ShockwaveReader input, ReaderContext context)
    {
        int version = input.ReadVarInt();
        if (version < 0x401) return;
        int unk1 = input.ReadVarInt();
        var versions = (DirectorVersion)input.ReadVarInt();

        if (version < 0x501) return;
        string versionString = input.ReadString();
    }

    public int GetBodySize(WriterOptions options)
    {
        throw new NotImplementedException();
    }
    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        throw new NotImplementedException();
    }

    public static FileVersion Read(ref ShockwaveReader input, ReaderContext context) => new FileVersion(ref input, context);
}
