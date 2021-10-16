using Shockky.IO;

namespace Shockky.Resources
{
    public class LingoContextItem : ShockwaveItem
    {
        public int ChunkIndex { get; set; }
        public LingoContextItemFlags Flags { get; set; }

        /// <summary>
        /// Points to next free context item.
        /// </summary>
        public short Link { get; set; }

        public LingoContextItem()
        { }
        public LingoContextItem(ref ShockwaveReader input)
        {
            input.ReadInt32();
            ChunkIndex = input.ReadInt32();
            Flags = (LingoContextItemFlags)input.ReadInt16();
            Link = input.ReadInt16();
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(int);
            size += sizeof(int);
            size += sizeof(short);
            size += sizeof(short);
            return size;
        }

        public override void WriteTo(ShockwaveWriter output)
        {
            output.Write(0);
            output.Write(ChunkIndex);
            output.Write((short)Flags);
            output.Write(Link);
        }
    }
}