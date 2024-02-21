using Shockky.IO;

namespace Shockky.Resources;

public sealed class LingoNames : IShockwaveItem, IResource
{
    public OsType Kind => OsType.Lnam;

    public List<string> Names { get; set; }

    public LingoNames()
    { }
    public LingoNames(ref ShockwaveReader input, ReaderContext context)
    {
        input.IsBigEndian = true;

        input.ReadInt32();
        input.ReadInt32();
        input.ReadInt32();
        input.ReadInt32();

        short nameOffset = input.ReadInt16();
        Names = new List<string>(input.ReadInt16());

        input.Position = nameOffset;
        for (int i = 0; i < Names.Capacity; i++)
        {
            Names.Add(input.ReadString());
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
        size += Names.Sum(n => n.Length + 1);
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        const short NAME_OFFSET = 20;
        int namesLength = Names?.Sum(static n => sizeof(byte) + n.Length) ?? 0;

        output.Write(0);
        output.Write(0);
        output.Write(namesLength);
        output.Write(namesLength);
        output.Write(NAME_OFFSET);
        output.Write((short)Names.Count);

        foreach (string name in Names)
        {
            output.Write(name);
        }
    }
}
