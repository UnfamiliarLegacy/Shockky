using Shockky.IO;

namespace Shockky.Resources.Cast
{
    public class XtraCastProperties : ShockwaveItem, IMemberProperties
    {
        public string SymbolName { get; set; }
        public byte[] Data { get; set; }

        public XtraCastProperties(ref ShockwaveReader input)
        {
            SymbolName = input.ReadString(input.ReadInt32());
            Data = new byte[input.ReadInt32()];
            input.ReadBytes(Data);
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(int);
            size += SymbolName.Length;
            size += sizeof(int);
            size += Data.Length;
            return size;
        }

        public override void WriteTo(ShockwaveWriter output)
        {
            throw new NotImplementedException(nameof(XtraCastProperties));
            output.Write(SymbolName.Length);
            output.Write(Data.Length);
            output.Write(Data);
        }
    }
}
