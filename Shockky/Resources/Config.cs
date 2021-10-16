using System.Drawing;

using Shockky.IO;
using Shockky.Resources.Cast;

namespace Shockky.Resources
{
    public class Config : Chunk
    {
        public const short LENGTH = 100;

        public DirectorVersion Version { get; set; }

        public Rectangle Rect { get; set; }

        public short MinMemberNum { get; }
        public short MaxMemberNum { get; }

        public short Field12 { get; set; }
        public short Field14 { get; set; }
        public short Field16 { get; set; }
        public short Field18 { get; set; }

        public byte LegacyTempo { get; set; }
        public bool LightSwitch { get; set; }

        public byte Field1E { get; set; }
        public byte Field1F { get; set; }
        public int Field20 { get; set; }

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

        public short Field3A { get; set; }
        public int Field3C { get; set; }
        public int Checksum { get; set; }

        public short OldDefaultPalette { get; set; }
        public int MaxCastResourceNum { get; set; }
        public CastMemberId DefaultPalette { get; set; }

        public int DownloadFramesBeforePlaying { get; set; }

        public Config()
            : base(ResourceKind.DRCF)
        { }
        public Config(ref ShockwaveReader input, ChunkHeader header)
            : base(header)
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
            Field14 = input.ReadInt16();
            Field16 = input.ReadInt16();
            Field18 = input.ReadInt16();
            // v >= 1025
            StagePalette = input.ReadInt16();
            DefaultColorDepth = input.ReadInt16();

            Field1E = input.ReadByte();
            Field1F = input.ReadByte();
            Field20 = input.ReadInt32();

            OriginalVersion = (DirectorVersion)input.ReadInt16();
            MaxCastColorDepth = input.ReadInt16();
            Flags = (ConfigFlags)input.ReadInt32();
            ScoreUsedChannelsMask = input.ReadUInt64();

            Trial = input.ReadBoolean();
            Field34 = input.ReadByte();

            Tempo = input.ReadInt16();
            Platform = (Platform)input.ReadInt16();
            // v >= 1113
            Field3A = input.ReadInt16();
            Field3C = input.ReadInt32();
            Checksum = input.ReadInt32();
            // v >= 1114
            OldDefaultPalette = input.ReadInt16();
            // v >= 1115
            Remnants.Enqueue(input.ReadInt16());

            MaxCastResourceNum = input.ReadInt32();
            DefaultPalette = new CastMemberId(input.ReadInt16(), input.ReadInt16());

            Remnants.Enqueue(input.ReadByte());
            Remnants.Enqueue(input.ReadByte());
            Remnants.Enqueue(input.ReadInt16());

            if (!input.IsDataAvailable) return;

            DownloadFramesBeforePlaying = input.ReadInt32();
            Remnants.Enqueue(input.ReadInt16());
            Remnants.Enqueue(input.ReadInt16());
            Remnants.Enqueue(input.ReadInt16());
            Remnants.Enqueue(input.ReadInt16());
            Remnants.Enqueue(input.ReadInt16());
            Remnants.Enqueue(input.ReadInt16());
        }

        public override void WriteBodyTo(ShockwaveWriter output)
        {
            output.Write(LENGTH);
            output.Write((short)Version);
            output.Write(Rect);
            output.Write(MinMemberNum);
            output.Write(MaxMemberNum);
            output.Write(LegacyTempo);
            output.Write(LightSwitch);

            output.Write(Field12);
            output.Write(Field14);
            output.Write(Field16);
            output.Write(Field18);

            output.Write(StagePalette);
            output.Write(DefaultColorDepth);

            output.Write(Field1E);
            output.Write(Field1F);
            output.Write(Field20);

            output.Write((short)OriginalVersion);
            output.Write(MaxCastColorDepth);
            output.Write((int)Flags);
            output.Write(ScoreUsedChannelsMask);
            output.Write(Trial);
            output.Write(Field34);
            output.Write(Tempo);
            output.Write((short)Platform);
            output.Write(Field3A);
            output.Write(Field3C);
            output.Write(Checksum);
            output.Write(OldDefaultPalette);

            output.Write((short)Remnants.Dequeue());
            output.Write(MaxCastResourceNum);
            output.Write(DefaultPalette);
            output.Write((byte)Remnants.Dequeue());
            output.Write((byte)Remnants.Dequeue());
            output.Write((short)Remnants.Dequeue());

            output.Write(DownloadFramesBeforePlaying);
            
            output.Write((short)Remnants.Dequeue());
            output.Write((short)Remnants.Dequeue());
            output.Write((short)Remnants.Dequeue());
            output.Write((short)Remnants.Dequeue());
            output.Write((short)Remnants.Dequeue());
            output.Write((short)Remnants.Dequeue());
        }

        public int CalculateChecksum()
        {
            return unchecked(
                (LENGTH + 1) *
                ((int)Version + 2) /
                (Rect.Top + 3) *
                (Rect.Left + 4) /
                (Rect.Bottom + 5) *
                (Rect.Right + 6) -
                (MinMemberNum + 7) *
                (MaxMemberNum + 8) -
                (LegacyTempo + 9) -
                ((LightSwitch ? 1 : 0) + 10) +
                (Field12 + 11) *
                (Field14 + 12) +
                (Field16 + 13) *
                (Field18 + 14) +
                (StagePalette + 15) +
                (DefaultColorDepth + 16) +
                (Field1E + 17) *
                (Field1F + 18) +
                (Field20 + 19) *
                ((int)OriginalVersion + 20) +
                (MaxCastColorDepth + 21) +
                ((int)Flags + 22) +
                ((int)(ScoreUsedChannelsMask >> 32) + 23) +
                ((int)(ScoreUsedChannelsMask & 0x0000FFFF) + 24) *
                (Field34 + 25) +
                (Tempo + 26) *
                ((int)Platform + 27) *
                ((Field3A * 3590) - 0xBB000)) ^ 0x7261_6C66;
        }

        public override int GetBodySize() => LENGTH;
    }
}
