using Shockky.IO;
using Shockky.Resources.Cast;
using Shockky.Resources.Cast.Properties;

namespace Shockky.Resources
{
    public class CastMemberProperties : Chunk
    {
        public MemberKind Type { get; set; }
        public CastMemberMetadata Metadata { get; set; }
        public IMemberProperties Properties { get; set; }

        public CastMemberProperties()
            : base(ResourceKind.CASt)
        { }
        public CastMemberProperties(ref ShockwaveReader input, ChunkHeader header)
            : base(header)
        {
            input.IsBigEndian = true;

            Type = (MemberKind)input.ReadInt32();
            input.ReadInt32();
            int dataLength = input.ReadInt32();

            //TODO: VWCI - MemberMetadata {
            Remnants.Enqueue(input.ReadInt32()); //pad 0
            Remnants.Enqueue(input.ReadInt32()); //script_ptrs
            Remnants.Enqueue(input.ReadInt32());
            Remnants.Enqueue(input.ReadInt32()); //flags
            Remnants.Enqueue(input.ReadInt32()); //script_context_num 
            Metadata = new CastMemberMetadata(ref input);
            // }

            Properties = ReadTypeProperties(ref input, dataLength);
        }

        private IMemberProperties ReadTypeProperties(ref ShockwaveReader input, int dataLength)
        {
            return Type switch
            {
                MemberKind.Bitmap or MemberKind.OLE => new BitmapCastProperties(ref input),
                MemberKind.FilmLoop or MemberKind.Movie => new FilmLoopCastProperties(ref input),
                MemberKind.Text => new TextCastProperties(ref input),
                MemberKind.Button => new ButtonCastProperties(ref input),
                MemberKind.Shape => new ShapeCastProperties(ref input),
                MemberKind.DigitalVideo => new VideoCastProperties(ref input),
                MemberKind.Script => new ScriptCastProperties(ref input),
                MemberKind.RichText => new RichTextCastProperties(ref input),
                MemberKind.Transition => new TransitionCastProperties(ref input),
                MemberKind.Xtra => new XtraCastProperties(ref input),

                _ => new UnknownCastProperties(ref input, dataLength)
            };
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(int);
            size += sizeof(int);
            size += sizeof(int);

            size += sizeof(int);
            size += sizeof(int);
            size += sizeof(int);
            size += sizeof(int);
            size += sizeof(int);

            size += Metadata.GetBodySize();
            size += Properties.GetBodySize();
            return size;
        }

        public override void WriteBodyTo(ShockwaveWriter output)
        {
            output.Write((int)Type);
            output.Write(Metadata.GetBodySize());
            output.Write(Properties.GetBodySize());

            output.Write((int)Remnants.Dequeue());
            output.Write((int)Remnants.Dequeue());
            output.Write((int)Remnants.Dequeue());
            output.Write((int)Remnants.Dequeue());
            output.Write((int)Remnants.Dequeue());

            Metadata.WriteTo(output);
            Properties.WriteTo(output);
        }
    }
}
