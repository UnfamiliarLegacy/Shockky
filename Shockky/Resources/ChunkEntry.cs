using System.Diagnostics;

using Shockky.IO;

namespace Shockky.Resources
{
    [DebuggerDisplay("{Kind} | Length: {Length}, Offset: {Offset}")]
    public class ChunkEntry : ShockwaveItem
    {
        public ResourceKind Kind { get; set; }
        public int Length { get; set; }
        public int Offset { get; set; }
        public ChunkEntryFlags Flags { get; set; }
        public short Unknown { get; set; }
        public int Link { get; set; }

        public ChunkEntry(ref ShockwaveReader input)
        {
            Kind = (ResourceKind)input.ReadBEInt32();
            Length = input.ReadBEInt32();
            Offset = input.ReadBEInt32();
            Flags = (ChunkEntryFlags)input.ReadBEInt16();
            Unknown = input.ReadBEInt16();
            Link = input.ReadBEInt32();
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(int);
            size += sizeof(int);
            size += sizeof(int);
            size += sizeof(short);
            size += sizeof(short);
            size += sizeof(int);
            return size;
        }

        public override void WriteTo(ShockwaveWriter output)
        {
            output.WriteBE((int)Kind);
            output.WriteBE(Length);
            output.WriteBE(Offset);
            output.WriteBE((short)Flags);
            output.WriteBE(Unknown);
            output.WriteBE(Link);
        }
    }
}