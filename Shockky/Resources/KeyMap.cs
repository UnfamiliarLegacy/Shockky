using Shockky.IO;
using Shockky.Resources.Types;

namespace Shockky.Resources;

public sealed class KeyMap : IShockwaveItem, IResource
{
    private const short ENTRY_SIZE = 12;

    public OsType Kind => OsType.KEYPtr;

    public Dictionary<ResourceId, int> ResourceMap { get; set; }

    public KeyMap()
    { }
    public KeyMap(ref ShockwaveReader input, ReaderContext context)
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
            OsType kind = (OsType)input.ReadBEInt32();

            if (!ResourceMap.TryAdd(new ResourceId(kind, memberId), index))
                throw new InvalidOperationException();
        }
    }

    public int GetBodySize(WriterOptions options)
    {
        int size = 0;
        size += sizeof(short);
        size += sizeof(short);
        size += sizeof(int);
        size += sizeof(int);
        size += ResourceMap.Count * ENTRY_SIZE;
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
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
}
