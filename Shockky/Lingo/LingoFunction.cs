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
        EnvironmentIndex = input.ReadInt16();
        EventKind = (LingoEventFlags)input.ReadInt16();

        Bytecode = new byte[input.ReadInt32()];
        int bytecodeOffset = input.ReadInt32();

        Arguments.Capacity = input.ReadInt16();
        int argumentsOffset = input.ReadInt32();

        Locals.Capacity = input.ReadInt16();
        int localsOffset = input.ReadInt32();

        short globalsCount = input.ReadInt16(); //v5
        int globalsOffset = input.ReadInt32(); //v5

        int parserBytesRead = input.ReadInt32();
        short bodyLineNumber = input.ReadInt16();

        BytesPerLine = new byte[input.ReadInt16()];
        int lineOffset = input.ReadInt32();

        //if version > 0x800
        StackHeight = input.ReadInt32();

        int handlerEndOffset = input.Position;

        input.Position = bytecodeOffset;
        input.ReadBytes(Bytecode);

        input.Position = argumentsOffset;
        for (int i = 0; i < Arguments.Capacity; i++)
        {
            Arguments.Add(input.ReadInt16());
        }

        input.Position = localsOffset;
        for (int i = 0; i < Locals.Capacity; i++)
        {
            Locals.Add(input.ReadInt16());
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
        output.Write(EnvironmentIndex);
        output.Write((short)EventKind);
    }
}
