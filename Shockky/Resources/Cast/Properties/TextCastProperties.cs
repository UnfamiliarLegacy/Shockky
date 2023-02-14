using System.Drawing;

using Shockky.IO;

namespace Shockky.Resources.Cast;

// TODO: D3Mac does extra stuff on loading for versions < 1026: - src: csnover
public class TextCastProperties : IMemberProperties
{
    public SizeType BorderSize { get; set; }
    public SizeType GutterSize { get; set; }
    public SizeType BoxShadowSize { get; set; }
    public TextBoxType BoxType { get; set; }

    public TextAlignment Alignment { get; set; }
    public Color BackgroundColor { get; set; }
    public short Font { get; set; }
    public Rectangle Rectangle { get; set; }
    public short LineHeight { get; set; }

    public SizeType TextShadowSize { get; set; }
    public TextFlags Flags { get; set; }

    public TextCastProperties()
    { }
    public TextCastProperties(ref ShockwaveReader input, ReaderContext context)
    {
        BorderSize = (SizeType)input.ReadByte();
        GutterSize = (SizeType)input.ReadByte();
        BoxShadowSize = (SizeType)input.ReadByte();
        BoxType = (TextBoxType)input.ReadByte();

        Alignment = (TextAlignment)input.ReadInt16();
        BackgroundColor = input.ReadColor();
        Font = input.ReadInt16();
        Rectangle = input.ReadRect();
        LineHeight = input.ReadInt16();

        TextShadowSize = (SizeType)input.ReadByte();
        Flags = (TextFlags)input.ReadByte();
    }

    public int GetBodySize(WriterOptions options)
    {
        int size = 0;
        size += sizeof(byte);
        size += sizeof(byte);
        size += sizeof(byte);
        size += sizeof(byte);
        size += sizeof(short);
        size += 6;
        size += sizeof(short);
        size += sizeof(short) * 4;
        size += sizeof(short);
        size += sizeof(byte);
        size += sizeof(byte);
        size += sizeof(short); //TODO:
        size += sizeof(short);
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        output.Write((byte)BorderSize);
        output.Write((byte)GutterSize);
        output.Write((byte)BoxShadowSize);
        output.Write((byte)BoxType);

        output.Write((short)Alignment);
        output.Write(BackgroundColor);

        output.Write(Font);
        output.Write(Rectangle);
        output.Write(LineHeight);

        output.Write((byte)TextShadowSize);
        output.Write((byte)Flags);
    }
}
