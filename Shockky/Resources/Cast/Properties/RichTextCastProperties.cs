using Shockky.IO;

using System.Drawing;

namespace Shockky.Resources.Cast;

public class RichTextCastProperties : IMemberProperties
{
    public Rectangle Rectangle { get; set; }
    public Rectangle Rect2 { get; set; }
    public bool AntiAlias { get; set; }
    public RichTextBoxType BoxType { get; set; }
    
    public short Unk12 { get; set; }
    public SizeType AntiAliasMinFontSize { get; set; }
    public SizeType Height { get; set; }
    
    public Color ForegroundColor { get; set; }
    public Color BackgroundColor { get; set; }

    public RichTextCastProperties(ref ShockwaveReader input)
    {
        Rectangle = input.ReadRect();
        Rect2 = input.ReadRect();
        AntiAlias = input.ReadBoolean(); 
        BoxType = (RichTextBoxType)input.ReadByte();

        Unk12 = input.ReadInt16();
        AntiAliasMinFontSize = (SizeType)input.ReadByte();
        Height = (SizeType)input.ReadByte();

        //TODO: Rgb24
        ForegroundColor = Color.FromArgb(input.ReadInt32());
        BackgroundColor = Color.FromArgb(input.ReadInt16(), input.ReadInt16(), input.ReadInt16());
    }
    public int GetBodySize(WriterOptions options)
    {
        int size = 0;
        size += sizeof(int) * 4;
        size += sizeof(int) * 4;
        size += sizeof(byte);
        size += sizeof(byte);
        size += sizeof(short);
        size += sizeof(byte);
        size += sizeof(byte);
        size += sizeof(int);
        size += sizeof(short) * 3;
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        output.Write(Rectangle);
        output.Write(Rect2);
    }
}
