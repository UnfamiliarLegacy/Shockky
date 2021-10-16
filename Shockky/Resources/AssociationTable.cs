using Shockky.IO;
using Shockky.Resources.Types;

namespace Shockky.Resources
{
    public class AssociationTable : Chunk
    {
        private const short ENTRY_SIZE = 12;
        
        public Dictionary<ResourceId, int> ResourceMap { get; set; }

        public AssociationTable()
            : base(ResourceKind.KEYPtr)
        { }
        public AssociationTable(ref ShockwaveReader input, ChunkHeader header)
            : base(header)
        {
            input.ReadInt16();
            input.ReadInt16();
            input.ReadInt32();
            int count = input.ReadBEInt32();
            ResourceMap = new Dictionary<ResourceId, int>(count);

            for (int i = 0; i < count; i++)
            {
                int index = input.ReadBEInt32();
                int memberId = input.ReadBEInt32();
                ResourceKind kind = (ResourceKind)input.ReadBEInt32();

                if (!ResourceMap.TryAdd(new ResourceId(kind, memberId), index))
                    throw new InvalidOperationException();
            }
        }

        public override void WriteBodyTo(ShockwaveWriter output)
        {
            output.WriteBE(ENTRY_SIZE);
            output.WriteBE(ENTRY_SIZE);
            output.WriteBE(ResourceMap?.Count ?? 0);
            output.WriteBE(ResourceMap?.Count ?? 0);
            foreach ((ResourceId resourceId, int index) in ResourceMap)
            {
                output.WriteBE(index);
                output.WriteBE(resourceId.Id);
                output.WriteBE((int)resourceId.Kind);
            }
        }

        public override int GetBodySize()
        {
            int size = 0;
            size += sizeof(short);
            size += sizeof(short);
            size += sizeof(int);
            size += sizeof(int);
            size += ResourceMap.Count * ENTRY_SIZE;
            return size;
        }
    }
}
