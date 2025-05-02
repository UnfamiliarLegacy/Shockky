using Shockky.IO;
using Shockky.Resources.Cast.Properties;
using Shockky.Resources.Enum;

namespace Shockky.Resources.Cast;

public sealed class CastMemberProperties : IResource, IShockwaveItem
{
    public OsType Kind => OsType.CASt;

    public MemberKind Type { get; set; }
    public CastMemberMetadata Metadata { get; set; }
    public IMemberProperties Properties { get; set; }

    public CastMemberProperties(ref ShockwaveReader input)
    {
        input.ReverseEndianness = false;

        Type = (MemberKind)input.ReadInt32BigEndian();
        int metadataLength = input.ReadInt32BigEndian();
        int propetiesLength = input.ReadInt32BigEndian();

        Metadata = new CastMemberMetadata(ref input);
        Properties = ReadTypeProperties(ref input, propetiesLength);
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
            // TODO: MemberKind.Xtra => new XtraCastProperties(ref input),

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
        output.WriteInt32LittleEndian((int)Type);
        output.WriteInt32LittleEndian(Metadata.GetBodySize(options));
        output.WriteInt32LittleEndian(Properties.GetBodySize(options));

        Metadata.WriteTo(output, options);
        Properties.WriteTo(output, options);
    }
}
