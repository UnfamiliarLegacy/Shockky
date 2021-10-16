using System.Text;

using Shockky.IO;

namespace Shockky.Resources
{
    public class ScoreLabels : Chunk
    {
        public Dictionary<short, string> Labels { get; set; }

        public ScoreLabels()
            : base(ResourceKind.VWLB)
        {
            Labels = new Dictionary<short, string>();
        }
        public ScoreLabels(ref ShockwaveReader input, ChunkHeader header)
            : base(header)
        {
            input.IsBigEndian = true;

            var offsetMap = new (short frame, int offset)[input.ReadInt16()];
            Labels = new Dictionary<short, string>(offsetMap.Length);

            for (int i = 0; i < offsetMap.Length; i++)
            {
                offsetMap[i] = (input.ReadInt16(), input.ReadInt16());
            }

            string labels = input.ReadString(input.ReadInt32());

            for (int i = 0; i < offsetMap.Length; i++)
            {
                var (frame, offset) = offsetMap[i];

                if (i == offsetMap.Length - 1)
                    Labels[frame] = labels[offset..];
                else
                    Labels[frame] = labels[offset..offsetMap[i + 1].offset];
            }
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(short);
            size += Labels.Count * (2 * sizeof(short));
            size += sizeof(int);
            size += Labels.Values.Sum(l => l.Length);
            return size;
        }

        public override void WriteBodyTo(ShockwaveWriter output)
        {
            int offset = 0;
            var builder = new StringBuilder();

            output.Write(Labels.Count);
            foreach ((short frame, string label) in Labels)
            {
                output.Write(frame);
                output.Write((short)offset);

                offset += label.Length;
                builder.Append(label);
            }

            output.Write(builder.Length);
            output.Write(Encoding.UTF8.GetBytes(builder.ToString()));
        }
    }
}