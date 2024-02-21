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

        Metadata = new CastMemberMetadata(ref input, context);
        Properties = ReadTypeProperties(ref input, context, dataLength);
    }

    private IMemberProperties ReadTypeProperties(ref ShockwaveReader input, ReaderContext context, int dataLength)
    {
        return Type switch
        {
            MemberKind.Bitmap or MemberKind.OLE => new BitmapCastProperties(ref input, context),
            MemberKind.FilmLoop or MemberKind.Movie => new FilmLoopCastProperties(ref input),
            MemberKind.Text => new TextCastProperties(ref input, context),
            MemberKind.Button => new ButtonCastProperties(ref input, context),
            MemberKind.Shape => new ShapeCastProperties(ref input, context),
            MemberKind.DigitalVideo => new VideoCastProperties(ref input, context),
            MemberKind.Script => new ScriptCastProperties(ref input, context),
            MemberKind.RichText => new RichTextCastProperties(ref input),
            MemberKind.Transition => new TransitionCastProperties(ref input, context),
            MemberKind.Xtra => new XtraCastProperties(ref input, context),

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

        size += Metadata.GetBodySize(options);
        size += Properties.GetBodySize(options);
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        output.Write((int)Type);
        output.Write(Metadata.GetBodySize(options));
        output.Write(Properties.GetBodySize(options));

        Metadata.WriteTo(output, options);
        Properties.WriteTo(output, options);
    }
}
