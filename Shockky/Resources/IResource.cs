﻿using Shockky.IO;

namespace Shockky.Resources;

public interface IResource
{
    OsType Kind { get; }

    public static IResource Read(scoped ref ShockwaveReader input, ReaderContext context)
    {
        var header = new ResourceHeader(ref input);
        return Read(ref input, context, header.Kind, header.Length);
    }
    public static IResource Read(scoped ref ShockwaveReader input, ReaderContext context, OsType kind, int length)
    {
        ReadOnlySpan<byte> chunkSpan = input.ReadBytes(length);
        var chunkInput = new ShockwaveReader(chunkSpan, input.ReverseEndianness);

        return kind switch
        {
            OsType.Fver => new FileVersion(ref chunkInput, context),
            OsType.Fcdr => new FileCompressionTypes(ref chunkInput, context),
            OsType.ABMP => new AfterburnerMap(ref chunkInput, context),

            OsType.imap => new IndexMap(ref chunkInput, context),
            OsType.mmap => new MemoryMap(ref chunkInput, context),
            OsType.KEYPtr => new KeyMap(ref chunkInput, context),
            OsType.VWCF or OsType.DRCF => new Config(ref chunkInput, context),

            // TODO: handle V1850
            //OsType.VWLB => new ScoreLabels(ref chunkInput, context),
            OsType.VWFI => new FileInfo(ref chunkInput, context),

            OsType.Lnam => new LingoNames(ref chunkInput, context),
            OsType.Lscr => new LingoScript(ref chunkInput),
            OsType.Lctx or OsType.LctX => new LingoContext(ref chunkInput, context),

            OsType.CASPtr => new CastMap(ref chunkInput, context),
            OsType.CASt => new CastMemberProperties(ref chunkInput, context),

            OsType.SCRF => new ScoreReference(ref chunkInput, context),
            OsType.Sord => new ScoreOrder(ref chunkInput, context),
            OsType.CLUT => new Palette(ref chunkInput, context),
            OsType.STXT => new StyledText(ref chunkInput, context),

            OsType.snd => new SoundData(ref chunkInput),

            OsType.Fmap => new FontMap(ref chunkInput, context),

            OsType.GRID => new Grid(ref chunkInput, context),
            OsType.FCOL => FavoriteColors.Read(ref chunkInput, context),

            OsType.BITD => new BitmapData(ref chunkInput, context),

            _ => new UnknownResource(ref chunkInput, context, kind)
        };
    }
}
