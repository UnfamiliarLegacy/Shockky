using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Buffers.Binary;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using Shockky.Resources;

namespace Shockky.IO;

// TODO: Eventually get rid of this wrapper and use extensions on ROS<byte>
public ref struct ShockwaveReader
{
    private readonly ReadOnlySpan<byte> _data;

    public bool IsBigEndian { get; set; }
    public int Position { get; set; }

    public readonly int Length => _data.Length;
    public readonly bool IsDataAvailable => Position < _data.Length;

    private readonly ReadOnlySpan<byte> CurrentSpan => _data.Slice(Position);

    public ShockwaveReader(ReadOnlySpan<byte> data, bool isBigEndian = false)
    {
        _data = data;

        Position = 0;
        IsBigEndian = isBigEndian;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count) => Position += count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadByte() => _data[Position++];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadBytes(int count)
    {
        ReadOnlySpan<byte> data = _data.Slice(Position, count);
        Advance(count);
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadBytes(Span<byte> buffer)
    {
        _data.Slice(Position, buffer.Length).CopyTo(buffer);
        Advance(buffer.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBoolean() => _data[Position++] == 1;

    //TODO: BigEndian => "ReverseEndian"
    public short ReadInt16()
    {
        short value = MemoryMarshal.Read<short>(CurrentSpan);
        Advance(sizeof(short));
    
        return IsBigEndian ? 
            BinaryPrimitives.ReverseEndianness(value) : value;
    }
    public short ReadBEInt16()
    {
        short value = MemoryMarshal.Read<short>(CurrentSpan);
        Advance(sizeof(short));

        return IsBigEndian ?
            value : BinaryPrimitives.ReverseEndianness(value);
    }

    public ushort ReadUInt16()
    {
        ushort value = MemoryMarshal.Read<ushort>(CurrentSpan);
        Advance(sizeof(ushort));

        return IsBigEndian ?
            BinaryPrimitives.ReverseEndianness(value) : value;
    }
    public ushort ReadBEUInt16()
    {
        ushort value = MemoryMarshal.Read<ushort>(CurrentSpan);
        Advance(sizeof(ushort));

        return IsBigEndian ?
            value : BinaryPrimitives.ReverseEndianness(value);
    }

    public int ReadInt32()
    {
        int value = MemoryMarshal.Read<int>(CurrentSpan);
        Advance(sizeof(int));

        return IsBigEndian ?
            BinaryPrimitives.ReverseEndianness(value) : value;
    }
    public int ReadBEInt32()
    {
        int value = MemoryMarshal.Read<int>(CurrentSpan);
        Advance(sizeof(int));

        return IsBigEndian ?
            value : BinaryPrimitives.ReverseEndianness(value);
    }

    public uint ReadUInt32()
    {
        uint value = MemoryMarshal.Read<uint>(CurrentSpan);
        Advance(sizeof(uint));

        return IsBigEndian ?
            BinaryPrimitives.ReverseEndianness(value) : value;
    }
    public uint ReadBEUInt32()
    {
        uint value = MemoryMarshal.Read<uint>(CurrentSpan);
        Advance(sizeof(uint));

        return IsBigEndian ?
            value : BinaryPrimitives.ReverseEndianness(value);
    }

    public ulong ReadUInt64()
    {
        ulong value = MemoryMarshal.Read<ulong>(CurrentSpan);
        Advance(sizeof(ulong));

        return IsBigEndian ?
            BinaryPrimitives.ReverseEndianness(value) : value;
    }
    public ulong ReadBEUInt64()
    {
        ulong value = MemoryMarshal.Read<ulong>(CurrentSpan);
        Advance(sizeof(ulong));

        return IsBigEndian ?
            value : BinaryPrimitives.ReverseEndianness(value);
    }
    public double ReadDouble()
    {
        double value = IsBigEndian ?
            BitConverter.Int64BitsToDouble(BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<long>(CurrentSpan))) :
            MemoryMarshal.Read<double>(CurrentSpan);

        Advance(sizeof(double));
        return value;
    }
    public double ReadBEDouble()
    {
        double value = IsBigEndian ?
            MemoryMarshal.Read<double>(CurrentSpan) :
            BitConverter.Int64BitsToDouble(BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<long>(CurrentSpan)));

        Advance(sizeof(double));
        return value;
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

    public string ReadString()
    {
        int length = ReadVarInt();
        return Encoding.UTF8.GetString(ReadBytes(length));
    }
    public string ReadString(int length)
    {
        return Encoding.UTF8.GetString(ReadBytes(length));
    }
    public string ReadNullString()
    {
        int length = CurrentSpan.IndexOf((byte)0);
        Debug.Assert(length != -1);
        string value = Encoding.UTF8.GetString(_data.Slice(Position, length));

        Position += length + 1;
        return value;
    }
    public string ReadInternationalString()
    {
        //Read 32-bit scalar values
        byte length = ReadByte();
        ReadOnlySpan<byte> data = ReadBytes(length * sizeof(int));

        return Encoding.UTF32.GetString(data);
    }

    public Color ReadColor()
    {
        // [R, R, G, G, B, B]
        byte r = _data[Position];
        byte g = _data[Position + 2];
        byte b = _data[Position + 4];

        Advance(6);
        return Color.FromArgb(r, g, b);
    }
    public Point ReadPoint()
    {
        return new(ReadInt16(), ReadInt16());
    }
    public Rectangle ReadRect()
    {
        short top = ReadInt16();
        short left = ReadInt16();
        short bottom = ReadInt16();
        short right = ReadInt16();

        return Rectangle.FromLTRB(left, top, right, bottom);
    }

    public unsafe IResource ReadCompressedResource(AfterburnerMapEntry entry, ReaderContext context)
    {
        Span<byte> decompressedData = entry.DecompressedLength <= 512 ?
                stackalloc byte[512] : new byte[entry.DecompressedLength];

        fixed (byte* dataPtr = _data)
        {
            using var stream = new UnmanagedMemoryStream(dataPtr, entry.Length);
            using var deflateStream = new ZLibStream(stream, CompressionMode.Decompress);

            deflateStream.ReadExactly(decompressedData);
        }
        Advance(entry.Length);

        var input = new ShockwaveReader(decompressedData.Slice(entry.DecompressedLength), IsBigEndian);
        return IResource.Read(ref input, context);
    }
}
