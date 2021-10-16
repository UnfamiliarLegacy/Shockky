using Shockky.IO;

namespace Shockky.Resources
{
    public class SoundData : BinaryData
    {
        public SoundData()
            : base(ResourceKind.snd)
        { }
        public SoundData(ref ShockwaveReader input, ChunkHeader header)
            : base(ref input, header)
        { }
    }
}
