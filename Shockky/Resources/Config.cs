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
        input.ReverseEndianness = true;

        input.ReadInt16LittleEndian();
        Version = (DirectorVersion)input.ReadInt16LittleEndian();

        Rect = input.ReadRect();

        MinMemberNum = input.ReadInt16LittleEndian();
        MaxMemberNum = input.ReadInt16LittleEndian();

        LegacyTempo = input.ReadByte();
        LightSwitch = input.ReadBoolean();

        Field12 = input.ReadInt16LittleEndian();

        CommentFont = input.ReadInt16LittleEndian();
        CommentSize = input.ReadInt16LittleEndian();
        CommentStyle = input.ReadInt16LittleEndian();
        // v >= 1025
        StagePalette = input.ReadInt16LittleEndian();
        DefaultColorDepth = input.ReadInt16LittleEndian();

        Field1E = input.ReadByte();
        Field1F = input.ReadByte();
        DataSize = input.ReadInt32LittleEndian();

        OriginalVersion = (DirectorVersion)input.ReadInt16LittleEndian();
        MaxCastColorDepth = input.ReadInt16LittleEndian();
        Flags = (ConfigFlags)input.ReadInt32LittleEndian();
        ScoreUsedChannelsMask = input.ReadUInt64();

        Trial = input.ReadBoolean();
        Field34 = input.ReadByte();

        Tempo = input.ReadInt16LittleEndian();
        Platform = (Platform)input.ReadInt16LittleEndian();
        // v >= 1113
        SaveSeed = input.ReadInt16LittleEndian();
        Field3C = input.ReadInt32LittleEndian();
        Checksum = input.ReadUInt32LittleEndian();
        // v >= 1114
        OldDefaultPalette = input.ReadInt16LittleEndian();
        // v >= 1115
        Field46 = input.ReadInt16LittleEndian();

        MaxCastResourceNum = input.ReadInt32LittleEndian();
        DefaultPalette = new CastMemberId(input.ReadInt16LittleEndian(), input.ReadInt16LittleEndian());

        Field50 = input.ReadByte();
        Field51 = input.ReadByte();
        Field52 = input.ReadInt16LittleEndian();

        if (!input.IsDataAvailable) return;

        DownloadFramesBeforePlaying = input.ReadInt32LittleEndian();
        Field58 = input.ReadInt16LittleEndian();
        Field5A = input.ReadInt16LittleEndian();
        Field5C = input.ReadInt16LittleEndian();
        Field5E = input.ReadInt16LittleEndian();
        Field60 = input.ReadInt16LittleEndian();
        Field62 = input.ReadInt16LittleEndian();
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
        output.WriteInt16LittleEndian(LENGTH);
        output.WriteInt16LittleEndian((short)Version);
        output.WriteRect(Rect);
        output.WriteInt16LittleEndian(MinMemberNum);
        output.WriteInt16LittleEndian(MaxMemberNum);
        output.WriteByte(LegacyTempo);
        output.WriteBoolean(LightSwitch);

        output.WriteInt16LittleEndian(Field12);
        output.WriteInt16LittleEndian(CommentFont);
        output.WriteInt16LittleEndian(CommentSize);
        output.WriteInt16LittleEndian(CommentStyle);

        output.WriteInt16LittleEndian(StagePalette);
        output.WriteInt16LittleEndian(DefaultColorDepth);

        output.WriteByte(Field1E);
        output.WriteByte(Field1F);
        output.WriteInt32LittleEndian(DataSize);

        output.WriteInt16LittleEndian((short)OriginalVersion);
        output.WriteInt16LittleEndian(MaxCastColorDepth);
        output.WriteInt32LittleEndian((int)Flags);
        output.WriteUInt64LittleEndian(ScoreUsedChannelsMask);
        output.WriteBoolean(Trial);
        output.WriteByte(Field34);
        output.WriteInt16LittleEndian(Tempo);
        output.WriteInt16LittleEndian((short)Platform);
        output.WriteInt16LittleEndian(SaveSeed);
        output.WriteInt32LittleEndian(Field3C);
        output.WriteUInt32LittleEndian(Checksum); // TODO: CalculateChecksum()
        output.WriteInt16LittleEndian(OldDefaultPalette);

        output.WriteInt16LittleEndian(Field46);
        output.WriteInt32LittleEndian(MaxCastResourceNum);
        output.WriteMemberId(DefaultPalette);
        output.WriteByte(Field50);
        output.WriteByte(Field51);
        output.WriteInt16LittleEndian(Field52);

        output.WriteInt32LittleEndian(DownloadFramesBeforePlaying);
        output.WriteInt16LittleEndian(Field58);
        output.WriteInt16LittleEndian(Field5A);
        output.WriteInt16LittleEndian(Field5C);
        output.WriteInt16LittleEndian(Field5E);
        output.WriteInt16LittleEndian(Field60);
        output.WriteInt16LittleEndian(Field62);
    }
}
