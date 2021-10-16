using Shockky.IO;

#nullable enable
namespace Shockky.Resources.Cast.Properties
{
    public class TransitionCastProperties : ShockwaveItem, IMemberProperties
    {
        public byte LegacyDuration { get; set; }
        public byte ChunkSize { get; set; }
        public TransitionType Type { get; set; }
        public TransitionFlags Flags { get; set; }
        public short DurationInMilliseconds { get; set; }

        public XtraCastProperties? Xtra { get; set; }

        public TransitionCastProperties(ref ShockwaveReader input)
        {
            //TODO: Version differences.
            LegacyDuration = input.ReadByte();
            ChunkSize = input.ReadByte();
            Type = (TransitionType)input.ReadByte();
            Flags = (TransitionFlags)input.ReadByte();
            DurationInMilliseconds = input.ReadInt16(); //TODO: Not in < D5

            if (!Flags.HasFlag(TransitionFlags.Standard))
                Xtra = new XtraCastProperties(ref input);
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(byte);
            size += sizeof(byte);
            size += sizeof(byte);
            size += sizeof(byte);
            size += sizeof(short);
            if (!Flags.HasFlag(TransitionFlags.Standard))
                size += Xtra!.GetBodySize();
            return size;
        }

        public override void WriteTo(ShockwaveWriter output)
        {
            throw new NotImplementedException();
        }
    }
}
