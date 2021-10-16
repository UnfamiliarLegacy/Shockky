using Shockky.IO;

namespace Shockky.Resources.Cast
{
    public class ScriptCastProperties : ShockwaveItem, IMemberProperties
    {
        public ScriptKind Kind { get; set; }

        public ScriptCastProperties()
        { }
        public ScriptCastProperties(ref ShockwaveReader input)
        {
            Kind = (ScriptKind)input.ReadInt16(); 
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(short);
            return size;
        }

        public override void WriteTo(ShockwaveWriter output)
        {
            output.Write((short)Kind);
        }
    }
}
