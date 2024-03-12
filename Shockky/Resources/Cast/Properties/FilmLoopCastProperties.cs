using Shockky.IO;

using System.Drawing;

namespace Shockky.Resources.Cast;

public class FilmLoopCastProperties : IMemberProperties
{
    public Rectangle Rectangle { get; set; }
    public FilmLoopFlags Flags { get; set; }

    public FilmLoopCastProperties()
    { }
    public FilmLoopCastProperties(ref ShockwaveReader input)
    {
        Rectangle = input.ReadRect();
        Flags = (FilmLoopFlags)input.ReadInt32LittleEndian();
        short unk14 = input.ReadInt16LittleEndian();
    }

    public int GetBodySize(WriterOptions options)
    {
        int size = 0;
        size += sizeof(short) * 4;
        size += sizeof(int);
        size += sizeof(short);
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        output.WriteRect(Rectangle);
        output.WriteInt32LittleEndian((int)Flags);
    }
}
