using Shockky.IO;

namespace Shockky.Resources;

public sealed class LingoContext : IShockwaveItem, IResource
{
    public OsType Kind => OsType.LctX;

    private const short SECTION_SIZE = 12;
    private const short EVENT_COUNT = 25;

    public List<LingoContextItem> Items { get; set; }
    public short[] EventHandlerNames { get; } = new short[25];

    public int Type { get; set; }
    public LingoContextFlags Flags { get; set; }

    public int ValuesChunkIndex { get; set; }
    public int NameChunkIndex { get; set; }

    public short ValidCount { get; set; }
    public short FreeChunkIndex { get; set; }

    public LingoContext()
    {
        Items = new List<LingoContextItem>();
    }
    public LingoContext(ref ShockwaveReader input, ReaderContext context)
    {
        input.IsBigEndian = true;

        input.ReadInt32();
        input.ReadInt32();

        Items = new List<LingoContextItem>(input.ReadInt32());
        input.ReadInt32();
        
        input.ReadInt16();
        input.ReadInt16();

        input.ReadInt32();
        Type = input.ReadInt32(); //TODO: ??

        ValuesChunkIndex = input.ReadInt32();
        NameChunkIndex = input.ReadInt32();

        ValidCount = input.ReadInt16(); 
        Flags = (LingoContextFlags)input.ReadInt16();
        FreeChunkIndex = input.ReadInt16();

        input.ReadInt16();
        input.ReadInt16(); //EnvIndex some_parent_maybe_index

        for (int i = 0; i < EVENT_COUNT; i++)
        {
            EventHandlerNames[i] = input.ReadInt16();
        }

        for (int i = 0; i < Items.Capacity; i++)
        {
            Items.Add(new LingoContextItem(ref input));
        }
    }

    public int GetBodySize(WriterOptions options)
    {
        int size = 0;
        size += sizeof(int);
        size += sizeof(int);
        size += sizeof(int);
        size += sizeof(int);
        size += sizeof(short);
        size += sizeof(short);
        size += sizeof(int);
        size += sizeof(int);
        size += sizeof(int);
        size += sizeof(int);
        size += sizeof(short);
        size += sizeof(short);
        size += sizeof(short);
        size += sizeof(short);
        size += sizeof(short);
        size += sizeof(short) * EVENT_COUNT;
        size += Items.Count * SECTION_SIZE;
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        const short ENTRY_OFFSET = 96;

        output.Write(0);
        output.Write(0);
        output.Write(Items.Count);
        output.Write(Items.Count);

        output.Write(ENTRY_OFFSET);
        output.Write(SECTION_SIZE);

        output.Write(0);
        output.Write(Type);

        output.Write(ValuesChunkIndex);
        output.Write(NameChunkIndex);

        output.Write(ValidCount);
        output.Write((short)Flags);
        output.Write(FreeChunkIndex);

        output.Write((short)-1);
        output.Write((short)-1);

        for (int i = 0; i < EVENT_COUNT; i++)
        {
            output.Write(EventHandlerNames[i]);
        }

        foreach (LingoContextItem section in Items)
        {
            section.WriteTo(output, options);
        }
    }

    public static LingoContext Read(ref ShockwaveReader input, ReaderContext context)
    {
        throw new NotImplementedException();
    }
}
