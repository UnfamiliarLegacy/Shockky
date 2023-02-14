using System.Drawing;

using Shockky.IO;
using Shockky.Resources.Cast;

namespace Shockky.Resources;

public sealed class Config : IShockwaveItem, IResource
{
    public OsType Kind => OsType.DRCF;

    public const short LENGTH = 100;

    public DirectorVersion Version { get; set; }

    public Rectangle Rect { get; set; }

    public short MinMemberNum { get; set; }
    public short MaxMemberNum { get; set; }

    public short Field12 { get; set; }
    public short CommentFont { get; set; }
    public short CommentSize { get; set; }
    public short CommentStyle { get; set; }

    public byte LegacyTempo { get; set; }
    public bool LightSwitch { get; set; }

    public byte Field1E { get; set; }
    public byte Field1F { get; set; }
    public int DataSize { get; set; }

    public short StagePalette { get; set; }
    public short DefaultColorDepth { get; set; }
    public DirectorVersion OriginalVersion { get; set; }
    public short MaxCastColorDepth { get; set; }
    public ConfigFlags Flags { get; set; }

    public ulong ScoreUsedChannelsMask { get; set; }
    public bool Trial { get; set; }
    public byte Field34 { get; set; }

    public short Tempo { get; set; }
    public Platform Platform { get; set; }

    public short SaveSeed { get; set; }
    public int Field3C { get; set; }
    public uint Checksum { get; set; }

    public short OldDefaultPalette { get; set; }
    public short Field46 { get; set; }
    public int MaxCastResourceNum { get; set; }
    public CastMemberId DefaultPalette { get; set; }
    public byte Field50 { get; set; }
    public byte Field51 { get; set; }
    public short Field52 { get; set; }

    public int DownloadFramesBeforePlaying { get; set; }
    public short Field58 { get; set; }
    public short Field5A { get; set; }
    public short Field5C { get; set; }
    public short Field5E { get; set; }
    public short Field60 { get; set; }
    public short Field62 { get; set; }

    public Config(ref ShockwaveReader input, ReaderContext context)
    {
        input.IsBigEndian = true;

        input.ReadInt16();
        Version = (DirectorVersion)input.ReadInt16();

        Rect = input.ReadRect();

        MinMemberNum = input.ReadInt16();
        MaxMemberNum = input.ReadInt16();

        LegacyTempo = input.ReadByte();
        LightSwitch = input.ReadBoolean();

        Field12 = input.ReadInt16();

        CommentFont = input.ReadInt16();
        CommentSize = input.ReadInt16();
        CommentStyle = input.ReadInt16();
        // v >= 1025
        StagePalette = input.ReadInt16();
        DefaultColorDepth = input.ReadInt16();

        Field1E = input.ReadByte();
        Field1F = input.ReadByte();
        DataSize = input.ReadInt32();

        OriginalVersion = (DirectorVersion)input.ReadInt16();
        MaxCastColorDepth = input.ReadInt16();
        Flags = (ConfigFlags)input.ReadInt32();
        ScoreUsedChannelsMask = input.ReadUInt64();

        Trial = input.ReadBoolean();
        Field34 = input.ReadByte();

        Tempo = input.ReadInt16();
        Platform = (Platform)input.ReadInt16();
        // v >= 1113
        SaveSeed = input.ReadInt16();
        Field3C = input.ReadInt32();
        Checksum = input.ReadUInt32();
        // v >= 1114
        OldDefaultPalette = input.ReadInt16();
        // v >= 1115
        Field46 = input.ReadInt16();

        MaxCastResourceNum = input.ReadInt32();
        DefaultPalette = new CastMemberId(input.ReadInt16(), input.ReadInt16());

        Field50 = input.ReadByte();
        Field51 = input.ReadByte();
        Field52 = input.ReadInt16();

        if (!input.IsDataAvailable) return;

        DownloadFramesBeforePlaying = input.ReadInt32();
        Field58 = input.ReadInt16();
        Field5A = input.ReadInt16();
        Field5C = input.ReadInt16();
        Field5E = input.ReadInt16();
        Field60 = input.ReadInt16();
        Field62 = input.ReadInt16();
    }

    public uint CalculateChecksum()
    {
        uint checksum = LENGTH + 1;
        unchecked
        {
            checksum *= ((uint)Version + 2);
            checksum /= (uint)Rect.Top + 3;
            checksum *= (uint)Rect.Left + 4;
            checksum /= (uint)Rect.Bottom + 5;
            checksum *= (uint)Rect.Right + 6;
            checksum -= (uint)MinMemberNum + 7;
            checksum *= (uint)MaxMemberNum + 8;
            checksum -= (uint)LegacyTempo + 9;
            checksum -= (uint)(LightSwitch ? 1 : 0) + 10;
            checksum += (uint)Field12 + 11;
            checksum *= (uint)CommentFont + 12;
            checksum += (uint)CommentSize + 13;
            checksum *= (uint)CommentStyle + 14;
            checksum += (uint)StagePalette + 15;
            checksum += (uint)DefaultColorDepth + 16;
            checksum += (uint)Field1E + 17;
            checksum *= (uint)Field1F + 18;
            checksum += (uint)DataSize + 19;
            checksum *= (uint)OriginalVersion + 20;
            checksum += (uint)MaxCastColorDepth + 21;
            checksum += (uint)Flags + 22;
            checksum += (uint)(ScoreUsedChannelsMask >> 32) + 23;
            checksum += (uint)(ScoreUsedChannelsMask & 0xFFFFFFFF) + 24;
            checksum *= (uint)Field34 + 25;
            checksum *= (uint)Tempo + 26;
            checksum *= (uint)Platform + 27;
            checksum *= (uint)((SaveSeed * 0xE06) + 0xFFF450000); // - 0xBB000
            checksum ^= 0x72616C66;
        }
        return checksum;
    }

    public int GetBodySize(WriterOptions options) => LENGTH;

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        output.Write(LENGTH);
        output.Write((short)Version);
        output.Write(Rect);
        output.Write(MinMemberNum);
        output.Write(MaxMemberNum);
        output.Write(LegacyTempo);
        output.Write(LightSwitch);

        output.Write(Field12);
        output.Write(CommentFont);
        output.Write(CommentSize);
        output.Write(CommentStyle);

        output.Write(StagePalette);
        output.Write(DefaultColorDepth);

        output.Write(Field1E);
        output.Write(Field1F);
        output.Write(DataSize);

        output.Write((short)OriginalVersion);
        output.Write(MaxCastColorDepth);
        output.Write((int)Flags);
        output.Write(ScoreUsedChannelsMask);
        output.Write(Trial);
        output.Write(Field34);
        output.Write(Tempo);
        output.Write((short)Platform);
        output.Write(SaveSeed);
        output.Write(Field3C);
        output.Write(Checksum); // TODO: CalculateChecksum()
        output.Write(OldDefaultPalette);

        output.Write(Field46);
        output.Write(MaxCastResourceNum);
        output.Write(DefaultPalette);
        output.Write(Field50);
        output.Write(Field51);
        output.Write(Field52);

        output.Write(DownloadFramesBeforePlaying);
        output.Write(Field58);
        output.Write(Field5A);
        output.Write(Field5C);
        output.Write(Field5E);
        output.Write(Field60);
        output.Write(Field62);
    }
}
