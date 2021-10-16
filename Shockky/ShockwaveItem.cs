using System.Buffers;

using Shockky.IO;

namespace Shockky
{
    public abstract class ShockwaveItem
    {
        public byte[] ToArray()
        {
            var arrayWriter = new ArrayBufferWriter<byte>(GetBodySize());
            WriteTo(arrayWriter);

            return arrayWriter.WrittenSpan.ToArray();
        }

        public void WriteTo(IBufferWriter<byte> output)
        {
            //TODO: How to approach (re)assembling files. Version? Endianness? Container?
            int size = GetBodySize();
            var writer = new ShockwaveWriter(output.GetSpan(size), isBigEndian: true);
            
            WriteTo(writer);
            output.Advance(size);
        }

        public abstract int GetBodySize();
        public abstract void WriteTo(ShockwaveWriter output);
    }
}