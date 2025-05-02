using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using Shockky.Resources.Cast;

namespace Shockky.IO;

public interface ShockwaveWriter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetVarIntSize(int value) => GetVarUIntSize((uint)value);
    
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

    void WriteBoolean(bool value);
    
    void WriteByte(byte value);
    
    void WriteBytes(ReadOnlySpan<byte> value);

    void WriteString(ReadOnlySpan<char> value);
    
    void WriteInt16LittleEndian(short value);
    
    void WriteInt16BigEndian(short value);
    
    void WriteUInt16LittleEndian(ushort value);
    
    void WriteUInt16BigEndian(ushort value);

    void Write7BitEncodedInt(int value);
    
    void Write7BitEncodedUInt(uint value);

    void WriteInt32LittleEndian(int value);
    
    void WriteInt32BigEndian(int value);

    void WriteUInt32LittleEndian(uint value);

    void WriteUInt32BigEndian(uint value);

    void WriteUInt64LittleEndian(ulong value);

    void WriteUInt64BigEndian(ulong value);

    void WriteColor(Color color);
    
    void WriteColor(byte r, byte g, byte b);
    
    void WritePoint(Point value);

    void WriteRect(Rectangle value);

    void WriteMemberIdLittleEndian(CastMemberId memberId);

    void WriteMemberIdBigEndian(CastMemberId memberId);
}
