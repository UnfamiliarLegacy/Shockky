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
        input.ReverseEndianness = true;

        input.ReadInt32LittleEndian();
        input.ReadInt32LittleEndian();

        Items = new List<LingoContextItem>(input.ReadInt32LittleEndian());
        input.ReadInt32LittleEndian();

        input.ReadInt16LittleEndian();
        input.ReadInt16LittleEndian();

        input.ReadInt32LittleEndian();
        Type = input.ReadInt32LittleEndian(); //TODO: ??

        ValuesChunkIndex = input.ReadInt32LittleEndian();
        NameChunkIndex = input.ReadInt32LittleEndian();

        ValidCount = input.ReadInt16LittleEndian();
        Flags = (LingoContextFlags)input.ReadInt16LittleEndian();
        FreeChunkIndex = input.ReadInt16LittleEndian();

        input.ReadInt16LittleEndian();
        input.ReadInt16LittleEndian(); //EnvIndex some_parent_maybe_index

        for (int i = 0; i < EVENT_COUNT; i++)
        {
            EventHandlerNames[i] = input.ReadInt16LittleEndian();
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

        output.WriteInt32LittleEndian(0);
        output.WriteInt32LittleEndian(0);
        output.WriteInt32LittleEndian(Items.Count);
        output.WriteInt32LittleEndian(Items.Count);

        output.WriteInt16LittleEndian(ENTRY_OFFSET);
        output.WriteInt16LittleEndian(SECTION_SIZE);

        output.WriteInt32LittleEndian(0);
        output.WriteInt32LittleEndian(Type);

        output.WriteInt32LittleEndian(ValuesChunkIndex);
        output.WriteInt32LittleEndian(NameChunkIndex);

        output.WriteInt16LittleEndian(ValidCount);
        output.WriteInt16LittleEndian((short)Flags);
        output.WriteInt16LittleEndian(FreeChunkIndex);

        output.WriteInt16LittleEndian((short)-1);
        output.WriteInt16LittleEndian((short)-1);

        for (int i = 0; i < EVENT_COUNT; i++)
        {
            output.WriteInt16LittleEndian(EventHandlerNames[i]);
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
