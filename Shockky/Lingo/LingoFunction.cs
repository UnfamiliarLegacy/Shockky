using Shockky.IO;

namespace Shockky.Lingo;

public class LingoFunction : IShockwaveItem
{
    public byte[] Bytecode { get; set; }
    public List<short> Arguments { get; }
    public List<short> Locals { get; }
    public byte[] BytesPerLine { get; }

    public short EnvironmentIndex { get; set; }
    public LingoEventFlags EventKind { get; }

    public int StackHeight { get; set; }

    public LingoFunction()
    {
        Arguments = new List<short>();
        Locals = new List<short>();
        BytesPerLine = Array.Empty<byte>();
    }
    public LingoFunction(ref ShockwaveReader input, ReaderContext context)
    {
        EnvironmentIndex = input.ReadInt16LittleEndian();
        EventKind = (LingoEventFlags)input.ReadInt16LittleEndian();

        Bytecode = new byte[input.ReadInt32LittleEndian()];
        int bytecodeOffset = input.ReadInt32LittleEndian();

        Arguments.Capacity = input.ReadInt16LittleEndian();
        int argumentsOffset = input.ReadInt32LittleEndian();

        Locals.Capacity = input.ReadInt16LittleEndian();
        int localsOffset = input.ReadInt32LittleEndian();

        short globalsCount = input.ReadInt16LittleEndian(); //v5
        int globalsOffset = input.ReadInt32LittleEndian(); //v5

        int parserBytesRead = input.ReadInt32LittleEndian();
        short bodyLineNumber = input.ReadInt16LittleEndian();

        BytesPerLine = new byte[input.ReadInt16LittleEndian()];
        int lineOffset = input.ReadInt32LittleEndian();

        //if version > 0x800
        StackHeight = input.ReadInt32LittleEndian();

        int handlerEndOffset = input.Position;

        input.Position = bytecodeOffset;
        input.ReadBytes(Bytecode);

        input.Position = argumentsOffset;
        for (int i = 0; i < Arguments.Capacity; i++)
        {
            Arguments.Add(input.ReadInt16LittleEndian());
        }

        input.Position = localsOffset;
        for (int i = 0; i < Locals.Capacity; i++)
        {
            Locals.Add(input.ReadInt16LittleEndian());
        }

        throw new NotImplementedException(nameof(LingoFunction));
    }

    public int GetBodySize(WriterOptions options)
    {
        int size = 0;
        size += sizeof(short);
        size += sizeof(short);
        size += sizeof(int);
        size += sizeof(int);
        size += sizeof(short);
        size += sizeof(int);
        size += sizeof(short);
        size += sizeof(int);
        size += sizeof(short);
        size += sizeof(int);
        size += sizeof(int);
        size += sizeof(short);
        size += sizeof(short);
        size += sizeof(int);

        size += sizeof(int);
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        throw new NotImplementedException();
        output.WriteInt16LittleEndian(EnvironmentIndex);
        output.WriteInt16LittleEndian((short)EventKind);
    }
}
