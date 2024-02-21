using System.Text;
using System.Drawing;
using System.Numerics;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using Shockky.Resources.Cast;

namespace Shockky.IO;

public ref struct ShockwaveWriter
{
    private int _position;
    private readonly bool _isBigEndian;
    private readonly Span<byte> _data;

    public readonly Span<byte> CurrentSpan => _data.Slice(_position);

    public ShockwaveWriter(Span<byte> data, bool isBigEndian)
    {
        _data = data;
        _position = 0;
        _isBigEndian = isBigEndian;
    }

    //TODO: Measure, with and without inlining
    //Advance? - Zero fill variant?

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count) => _position += count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(byte value) => _data[_position++] = value;

    public void Write(ReadOnlySpan<byte> value)
    {
        value.CopyTo(_data.Slice(_position));
        _position += value.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(bool value)
    {
        _data[_position++] = Unsafe.As<bool, byte>(ref value);
    }

    public void Write(short value)
    {
        if (_isBigEndian)
        {
            value = BinaryPrimitives.ReverseEndianness(value);
        }

        MemoryMarshal.Write(_data.Slice(_position), ref value);
        _position += sizeof(short);
    }
    public void WriteBE(short value)
    {
        if (!_isBigEndian)
        {
            value = BinaryPrimitives.ReverseEndianness(value);
        }

        MemoryMarshal.Write(_data.Slice(_position), ref value);
        _position += sizeof(short);
    }

    public void Write(ushort value)
    {
        if (_isBigEndian)
        {
            value = BinaryPrimitives.ReverseEndianness(value);
        }

        MemoryMarshal.Write(_data.Slice(_position), ref value);
        _position += sizeof(ushort);
    }
    public void WriteBE(ushort value)
    {
        if (!_isBigEndian)
        {
            value = BinaryPrimitives.ReverseEndianness(value);
        }

        MemoryMarshal.Write(_data.Slice(_position), ref value);
        _position += sizeof(ushort);
    }

    public void Write(int value)
    {
        if (_isBigEndian)
        {
            value = BinaryPrimitives.ReverseEndianness(value);
        }

        MemoryMarshal.Write(_data.Slice(_position), ref value);
        _position += sizeof(int);
    }
    public void WriteBE(int value)
    {
        if (!_isBigEndian)
        {
            value = BinaryPrimitives.ReverseEndianness(value);
        }

        MemoryMarshal.Write(_data.Slice(_position), ref value);
        _position += sizeof(int);
    }

    public void Write(uint value)
    {
        if (_isBigEndian)
        {
            value = BinaryPrimitives.ReverseEndianness(value);
        }

        MemoryMarshal.Write(_data.Slice(_position), ref value);
        _position += sizeof(uint);
    }
    public void WriteBE(uint value)
    {
        if (!_isBigEndian)
        {
            value = BinaryPrimitives.ReverseEndianness(value);
        }

        MemoryMarshal.Write(_data.Slice(_position), ref value);
        _position += sizeof(uint);
    }

    public void Write(ulong value)
    {
        if (_isBigEndian)
        {
            value = BinaryPrimitives.ReverseEndianness(value);
        }

        MemoryMarshal.Write(_data.Slice(_position), ref value);
        _position += sizeof(ulong);
    }
    public void WriteBE(ulong value)
    {
        if (!_isBigEndian)
        {
            value = BinaryPrimitives.ReverseEndianness(value);
        }

        MemoryMarshal.Write(_data.Slice(_position), ref value);
        _position += sizeof(ulong);
    }

    public void WriteVarInt(int value) => WriteVarUInt((uint)value);
    public void WriteVarUInt(uint value)
    {
        int size = GetVarUIntSize(value);

        ref byte bufferPtr = ref MemoryMarshal.GetReference(_data);
        bufferPtr = Unsafe.Add(ref bufferPtr, _position);

        if (_position + size <= _data.Length)
        {
            switch (((31 - (uint)BitOperations.LeadingZeroCount(value | 1)) * 37) >> 8)
            {
                case 0:
                    bufferPtr = (byte)value;
                    _position += 1;
                    return;
                case 1:
                    bufferPtr = (byte)(value | 0x80);
                    Unsafe.Add(ref bufferPtr, 1) = (byte)(value >> 7);
                    _position += 2;
                    return;
                case 2:
                    bufferPtr = (byte)(value | 0x80);
                    Unsafe.Add(ref bufferPtr, 1) = (byte)((value >> 7) | 0x80);
                    Unsafe.Add(ref bufferPtr, 2) = (byte)(value >> 14);
                    _position += 3;
                    return;
                case 3:
                    bufferPtr = (byte)(value | 0x80);
                    Unsafe.Add(ref bufferPtr, 1) = (byte)((value >> 7) | 0x80);
                    Unsafe.Add(ref bufferPtr, 2) = (byte)((value >> 14) | 0x80);
                    Unsafe.Add(ref bufferPtr, 3) = (byte)(value >> 21);
                    _position += 5;
                    return;
                default:
                    bufferPtr = (byte)(value | 0x80);
                    Unsafe.Add(ref bufferPtr, 1) = (byte)((value >> 7) | 0x80);
                    Unsafe.Add(ref bufferPtr, 2) = (byte)((value >> 14) | 0x80);
                    Unsafe.Add(ref bufferPtr, 3) = (byte)((value >> 21) | 0x80);
                    Unsafe.Add(ref bufferPtr, 4) = (byte)(value >> 28);
                    _position += 5;
                    return;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetVarIntSize(int value)
    {
        return GetVarUIntSize((uint)value);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetVarUIntSize(uint value)
    {
        // bits_to_encode = (data != 0) ? 32 - CLZ(x) : 1  // 32 - CLZ(data | 1) 
        // bytes = ceil(bits_to_encode / 7.0);             // (6 + bits_to_encode) / 7
        int x = 6 + 32 - BitOperations.LeadingZeroCount(value | 1);
        // Division by 7 is done by (x * 37) >> 8 where 37 = ceil(256 / 7).
        // This works for 0 <= x < 256 / (7 * 37 - 256), i.e. 0 <= x <= 85.
        return (x * 37) >> 8;
    }

    public void Write(ReadOnlySpan<char> value)
    {
        WriteVarUInt((uint)value.Length);

        int len = Encoding.UTF8.GetBytes(value, _data.Slice(_position));
        _position += len;
    }
    public void WriteCString(ReadOnlySpan<char> value)
    {
        int len = Encoding.UTF8.GetBytes(value, _data.Slice(_position));
        _data[_position + len] = 0;
        _position += len + 1;
    }

    public void Write(Color color) => Write(color.R, color.G, color.B);
    public void Write(byte r, byte g, byte b)
    {
        // Eliminate bounds-checks.
        var buffer = _data.Slice(_position, 6);

        buffer[0] = r;
        buffer[1] = r;

        buffer[2] = g;
        buffer[3] = g;

        buffer[4] = b;
        buffer[5] = b;
    }
    public void Write(Point value)
    {
        Write((short)value.X);
        Write((short)value.Y);
    }
    public void Write(Rectangle value)
    {
        Write((short)value.Top);
        Write((short)value.Left);
        Write((short)value.Bottom);
        Write((short)value.Right);
    }
    public void Write(CastMemberId memberId)
    {
        Write(memberId.CastLib);
        Write(memberId.MemberNum);
    }
}
