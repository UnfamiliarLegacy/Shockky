using Shockky.IO;

using System.Diagnostics;

namespace Shockky.Resources
{
    [DebuggerDisplay("{Kind}")]
    public class ChunkHeader
    {
        public bool IsVariableLength => Kind switch
        {
            ResourceKind.Fver or
            ResourceKind.Fcdr or
            ResourceKind.ABMP or
            ResourceKind.FGEI => true,

            _ => false
        };

        public ResourceKind Kind { get; set; }
        public int Length { get; set; }

        public ChunkHeader(ResourceKind kind)
        {
            Kind = kind;
        }
        public ChunkHeader(ref ShockwaveReader input)
            : this((ResourceKind)input.ReadBEInt32())
        {
            Length = IsVariableLength ? 
                input.ReadVarInt() : input.ReadBEInt32();
        }

        public int GetBodySize()
        {
            int size = 0;
            size += sizeof(int);
            size += IsVariableLength ? ShockwaveWriter.GetVarIntSize(Length) : sizeof(int);
            return size;
        }

        public void WriteTo(ShockwaveWriter output)
        {
            output.WriteBE((int)Kind);
            if (IsVariableLength)
            {
                output.WriteVarInt(Length);
            }
            else output.WriteBE(Length);
        }
    }
}
