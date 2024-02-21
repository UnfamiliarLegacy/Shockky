using Shockky.IO;

namespace Shockky.Resources;

/// <summary>
/// Represents the guide grid in the Director.
/// </summary>
public sealed class Grid : IShockwaveItem, IResource
{
    public OsType Kind => OsType.GRID;

    public int Unknown { get; set; }
    public short Width { get; set; }
    public short Height { get; set; }
    public GridDisplay Display { get; set; }
    public short GridColor { get; set; }

    public short GuideColor { get; set; }
    public Guide[] Guides { get; set; }

    public Grid(ref ShockwaveReader input, ReaderContext context)
    {
        input.IsBigEndian = true;

        Unknown = input.ReadInt32();

        Width = input.ReadInt16();
        Height = input.ReadInt16();
        Display = (GridDisplay)input.ReadInt16();
        GridColor = input.ReadInt16();

        Guides = new Guide[input.ReadInt16()];
        GuideColor = input.ReadInt16();
        for (int i = 0; i < Guides.Length; i++)
        {
            Guides[i] = Guide.Read(ref input, context);
        }
    }

    public int GetBodySize(WriterOptions options)
    {
        const int GUIDE_ENTRY_SIZE = sizeof(short) + sizeof(short);

        int size = 0;
        size += sizeof(int);

        size += sizeof(short);
        size += sizeof(short);
        size += sizeof(short); 
        size += sizeof(short);

        size += sizeof(short);
        size += sizeof(short);
        size += Guides.Length * GUIDE_ENTRY_SIZE;
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        output.Write(Unknown);

        output.Write(Height);
        output.Write(Width);
        output.Write((short)Display);
        output.Write(GridColor);

        output.Write((short)Guides.Length);
        output.Write(GuideColor);
        foreach (Guide guide in Guides)
        {
            guide.WriteTo(output, options);
        }
    }
}
