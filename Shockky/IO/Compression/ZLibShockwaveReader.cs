using System.Text;
using System.Buffers.Binary;
using System.IO.Compression;

namespace Shockky.IO;

public sealed class ZLibShockwaveReader : BinaryReader
{
    private readonly bool _isBigEndian;

    public ZLibShockwaveReader(Stream innerStream, bool isBigEndian, bool leaveOpen)
        : base(new ZLibStream(innerStream, CompressionMode.Decompress, leaveOpen))
    {
        _isBigEndian = isBigEndian;
    }

    public int ReadVarInt()
    {
        int value = 0;
        byte b;
        do
        {
            b = ReadByte();
            value = (value << 7) + (b & 0x7F);
        }
        while (b >> 7 != 0);
        return value;
    }

    public string ReadCString()
    {
        char currentChar;
        StringBuilder builder = new();
        while ((currentChar = ReadChar()) != '\0')
        {
            builder.Append(currentChar);
        }
        return builder.ToString();
    }

    public new short ReadInt16()
    {
        return _isBigEndian ? BinaryPrimitives.ReverseEndianness(base.ReadInt16()) : base.ReadInt16();
    }
    public short ReadBEInt16()
    {
        return _isBigEndian ? base.ReadInt16() : BinaryPrimitives.ReverseEndianness(base.ReadInt16());
    }

    public new int ReadInt32()
    {
        return _isBigEndian ? BinaryPrimitives.ReverseEndianness(base.ReadInt32()) : base.ReadInt32();
    }
    public int ReadBEInt32()
    {
        return _isBigEndian ? base.ReadInt32() : BinaryPrimitives.ReverseEndianness(base.ReadInt32());
    }
}
