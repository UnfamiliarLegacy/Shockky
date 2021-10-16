using Shockky.IO;

namespace Shockky.Resources.Cast
{
    public class UnknownCastProperties : ShockwaveItem, IMemberProperties
    {
        public byte[] Data { get; set; }

        public UnknownCastProperties(ref ShockwaveReader input, int length)
        {
            Data = input.ReadBytes(length).ToArray();
        }

        public override int GetBodySize() => Data.Length;
        public override void WriteTo(ShockwaveWriter output) => output.Write(Data);
    }
}
