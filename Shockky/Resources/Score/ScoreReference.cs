using Shockky.IO;

namespace Shockky.Resources;

public sealed class ScoreReference : IShockwaveItem, IResource
{
    public OsType Kind => OsType.SCRF;

    public int Unknown { get; set; }
    public Dictionary<short, int> Entries { get; }

    public ScoreReference()
    { }
    public ScoreReference(ref ShockwaveReader input, ReaderContext context)
    {
        input.ReadInt32();
        input.ReadInt32();

        int entryCount = input.ReadInt32();
        input.ReadInt32();

        input.ReadInt16();
        input.ReadInt16();

        Unknown = input.ReadInt32();

        Entries = new Dictionary<short, int>(entryCount);
        for (int i = 0; i < entryCount; i++)
        {
            Entries.Add(input.ReadInt16(), input.ReadInt32());
        }
    }

    public int GetBodySize(WriterOptions options)
    {
        int size = 0;
        size += sizeof(int);
        size += sizeof(int);
        size += sizeof(int);
        size += sizeof(int);
        size += sizeof(short);
        size += sizeof(short);
        size += sizeof(int);
        size += Entries.Count * (sizeof(short) + sizeof(int));
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        const short ENTRY_OFFSET = 24;
        const short UNKNOWN = 8;

        output.Write(0);
        output.Write(0);

        output.Write(Entries.Count);
        output.Write(Entries.Count);

        output.Write(ENTRY_OFFSET);
        output.Write(UNKNOWN);

        output.Write(Unknown);
        foreach ((short number, int castMapPtrId) in Entries)
        {
            output.Write(number);
            output.Write(castMapPtrId);
        }
    }
}
