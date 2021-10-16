using Shockky.IO;

namespace Shockky.Resources
{
    public class Channel : ShockwaveItem
    {
        public byte ForeColor { get; set; }
        public byte BackColor { get; set; }

        public short LocV { get; set; } //x
        public short LocH { get; set; } //Y

        public short Height { get; set; }
        public short Width { get; set; }

        public bool FlipV { get; set; }
        public bool FlipH { get; set; }

        public Channel(ref ShockwaveReader input)
        { }

        public override int GetBodySize()
        {
            throw new NotImplementedException();
        }

        public override void WriteTo(ShockwaveWriter output)
        {
            throw new NotImplementedException();
        }
    }
}
