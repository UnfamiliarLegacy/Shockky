using Shockky.IO;

namespace Shockky.Resources
{
    public class StyledText : Chunk
    {
        public string Text { get; set; }
        public TextFormat[] Formats { get; set; }

        public StyledText()
            : base(ResourceKind.STXT)
        {
            Formats = Array.Empty<TextFormat>();
        }
        public StyledText(ref ShockwaveReader input, ChunkHeader header)
            : base(header)
        {
            input.IsBigEndian = true;

            input.ReadInt32();
            int textLength = input.ReadInt32();
            input.ReadInt32();

            Text = input.ReadString(textLength);

            Formats = new TextFormat[input.ReadInt16()];
            for (int i = 0; i < Formats.Length; i++)
            {
                Formats[i] = new TextFormat(ref input);
            }
        }

        public override void WriteBodyTo(ShockwaveWriter output)
        {
            const int TEXT_OFFSET = 12;
            const int TEXT_FORMAT_SIZE = 20;

            output.Write(TEXT_OFFSET);
            output.Write(Text.Length);
            output.Write(sizeof(short) + (Formats.Length * TEXT_FORMAT_SIZE));

            output.Write(Text); //TODO: 

            output.Write((short)Formats.Length);
            for (int i = 0; i < Formats.Length; i++)
            {
                Formats[i].WriteTo(output);
            }
        }

        public override int GetBodySize()
        {
            const int TEXT_FORMAT_SIZE = 20;

            int size = 0;
            size += sizeof(int);
            size += sizeof(int);
            size += sizeof(int);
            size += Text.Length;
            size += sizeof(short);
            size += Formats.Length * TEXT_FORMAT_SIZE;
            return size;
        }
    }
}
