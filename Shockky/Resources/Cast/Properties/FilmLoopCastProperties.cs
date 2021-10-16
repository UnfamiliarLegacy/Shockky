using Shockky.IO;

using System.Drawing;

namespace Shockky.Resources.Cast
{
    public class FilmLoopCastProperties : ShockwaveItem, IMemberProperties
    {
        public Rectangle Rectangle { get; set; }
        public FilmLoopFlags Flags { get; set; }

        public FilmLoopCastProperties()
        { }
        public FilmLoopCastProperties(ref ShockwaveReader input)
        {
            Rectangle = input.ReadRect();
            Flags = (FilmLoopFlags)input.ReadInt32();
            short unk14 = input.ReadInt16();
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(short) * 4;
            size += sizeof(int);
            size += sizeof(short);
            return size;
        }

        public override void WriteTo(ShockwaveWriter output)
        {
            output.Write(Rectangle);
            output.Write((int)Flags);
        }
    }
}
