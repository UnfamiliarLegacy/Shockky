using System.Drawing;

using Shockky.IO;

namespace Shockky.Resources;

public sealed class TextFormat : IShockwaveItem
{
    public int Offset { get; set; }
    public short Height { get; set; }
    public short Ascent { get; set; }
    public short FontId { get; set; }
    public bool Slant { get; set; }
    public byte Padding { get; set; }
    public short FontSize { get; set; }
    public Color Color { get; set; }

    public TextFormat(ref ShockwaveReader input, ReaderContext context)
    {
        Offset = input.ReadInt32();
        Height = input.ReadInt16();
        Ascent = input.ReadInt16();
        FontId = input.ReadInt16();
        Slant = input.ReadBoolean();
        Padding = input.ReadByte();
        FontSize = input.ReadInt16();
        Color = input.ReadColor();
    }

    public int GetBodySize(WriterOptions options)
    {
        int size = 0;
        size += sizeof(int);
        size += sizeof(short);
        size += sizeof(short);
        size += sizeof(short);
        size += sizeof(bool);
        size += sizeof(byte);
        size += sizeof(short);
        size += sizeof(short);
        size += sizeof(short);
        size += sizeof(short);
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        output.Write(Offset);
        output.Write(Height);
        output.Write(Ascent);
        output.Write(FontId);
        output.Write(Slant);
        output.Write(Padding);
        output.Write(FontSize);
        output.Write(Color);
    }
}
