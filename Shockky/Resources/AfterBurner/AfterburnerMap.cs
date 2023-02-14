using Shockky.IO;

namespace Shockky.Resources;

public sealed class AfterburnerMap : IShockwaveItem, IResource
{
    public OsType Kind => OsType.ABMP;

    public int Unknown { get; set; }
    public int Unknown2 { get; set; }
    public int LastIndex { get; set; }
    public Dictionary<int, AfterburnerMapEntry> Entries { get; set; }

    public AfterburnerMap(ref ShockwaveReader input, ReaderContext context)
    {
        input.ReadByte();
        Unknown = input.ReadVarInt();

        using ZLibShockwaveReader deflaterInput = IResource.CreateDeflateReader(ref input);
        Unknown2 = deflaterInput.ReadVarInt();
        LastIndex = deflaterInput.ReadVarInt();

        int count = deflaterInput.ReadVarInt();
        var entries = new Dictionary<int, AfterburnerMapEntry>(count);
        for (int i = 0; i < count; i++)
        {
            entries[i] = new AfterburnerMapEntry(deflaterInput);
        }
    }

    public int GetBodySize(WriterOptions options) => throw new NotImplementedException();
    public void WriteTo(ShockwaveWriter output, WriterOptions options) => throw new NotImplementedException();
}
