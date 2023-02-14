using Shockky.IO;
using Shockky.Resources.Cast;
using Shockky.Resources.Cast.Properties;

namespace Shockky.Resources;

public sealed class CastMemberProperties : IResource, IShockwaveItem
{
    public OsType Kind => OsType.CASt;

    public MemberKind Type { get; set; }
    public CastMemberMetadata Metadata { get; set; }
    public IMemberProperties Properties { get; set; }

    public CastMemberProperties(ref ShockwaveReader input, ReaderContext context)
    {
        input.IsBigEndian = true;

        Type = (MemberKind)input.ReadInt32();
        int ciLength = input.ReadInt32();
        int dataLength = input.ReadInt32();

        Metadata = CastMemberMetadata.Read(ref input, context);
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

    public int GetBodySize(WriterOptions options)
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

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        output.Write((int)Type);
        output.Write(Metadata.GetBodySize());
        output.Write(Properties.GetBodySize(options));

        Metadata.WriteTo(output);
        Properties.WriteTo(output);
    }
}
