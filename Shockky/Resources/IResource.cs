using Shockky.IO;
using Shockky.Resources.AfterBurner;
using Shockky.Resources.Cast;
using Shockky.Resources.Enum;
using Shockky.Resources.Lingo;
using Shockky.Resources.Score;

namespace Shockky.Resources;

public interface IResource
{
    OsType Kind { get; }

    public static IResource Read(scoped ref ShockwaveReader input)
    {
        var header = new ResourceHeader(ref input);
        return Read(ref input, header.Kind, header.Length);
    }
    public static IResource Read(scoped ref ShockwaveReader input, OsType kind, int length)
    {
        ReadOnlySpan<byte> chunkSpan = input.ReadBytes(length);
        var bodyInput = new ShockwaveReader(chunkSpan, input.ReverseEndianness);

        return kind switch
        {
            OsType.Fver => new FileVersion(ref bodyInput),
            OsType.Fcdr => new FileCompressionTypes(ref bodyInput),
            OsType.ABMP => new AfterburnerMap(ref bodyInput),

            OsType.imap => new IndexMap(ref bodyInput),
            OsType.mmap => new MemoryMap(ref bodyInput),
            OsType.KEYPtr => new KeyMap(ref bodyInput),
            // TODO: Fix crash on "hh_room_starlounge bug.cct".
            // OsType.VWCF or OsType.DRCF => new Config(ref bodyInput),

            // TODO: handle V1850
            //OsType.VWLB => new ScoreLabels(ref chunkInput),
            OsType.VWFI => new FileInfo(ref bodyInput),

            OsType.Lnam => new LingoNames(ref bodyInput),
            OsType.Lscr => new LingoScript(ref bodyInput),
            OsType.Lctx or OsType.LctX => new LingoContext(ref bodyInput),

            OsType.CASPtr => new CastMap(ref bodyInput),
            OsType.CASt => new CastMemberProperties(ref bodyInput),

            OsType.SCRF => new ScoreReference(ref bodyInput),
            OsType.Sord => new ScoreOrder(ref bodyInput),
            OsType.CLUT => new Palette(ref bodyInput),
            // TODO: Fix crash on "hh_bbinterface.cct".
            // OsType.STXT => new StyledText(ref bodyInput),

            OsType.snd => new SoundData(ref bodyInput),

            OsType.Fmap => new FontMap(ref bodyInput),

            OsType.GRID => new Grid(ref bodyInput),
            OsType.FCOL => FavoriteColors.Read(ref bodyInput),

            OsType.BITD => new BitmapData(ref bodyInput),

            _ => new UnknownResource(ref bodyInput, kind)
        };
    }
}
