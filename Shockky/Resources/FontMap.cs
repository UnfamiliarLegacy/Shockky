using Shockky.IO;

namespace Shockky.Resources
{
    public class FontMap : BinaryData
    {
        public FontMap()
            : base(ResourceKind.FXmp)
        { }
        public FontMap(ref ShockwaveReader input, ChunkHeader header)
            : base(ref input, header)
        { }
    }
}
