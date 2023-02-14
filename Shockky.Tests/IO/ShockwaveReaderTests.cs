using Shockky.IO;

using Xunit;

namespace Shockky.Tests.IO;

public class ShockwaveReaderTests
{
    //TODO: Cover rest of the reader implementation

    [Fact]
    public void Read_ValidVarInts()
    {
        Span<byte> encodedValueBuffer = stackalloc byte[] {
            0x00,                     // 0
            0x01,                     // 1 
            0x7F,                     // 127
            0x81, 0x00,               // 128
            0xC0, 0x00,               // 8192
            0xFF, 0x7F,               // 16383
            0x81, 0x80, 0x00,         // 16384
            0xFF, 0xFF, 0x7F,         // 2097151
            0x81, 0x80, 0x80, 0x00,   // 2097152
            0xC0, 0x80, 0x80, 0x00,   // 134217728
            0xFF, 0xFF, 0xFF, 0x7F,   // 268435455
            0xC0, 0x00,               // 8192
            0x86, 0xB0, 0x8F, 0x49    // 13371337
        };

        ShockwaveReader input = new(encodedValueBuffer);

        Assert.Equal(0, input.ReadVarInt());
        Assert.Equal(1, input.ReadVarInt());
        Assert.Equal(127, input.ReadVarInt());
        Assert.Equal(128, input.ReadVarInt());
        Assert.Equal(8192, input.ReadVarInt());
        Assert.Equal(16383, input.ReadVarInt());
        Assert.Equal(16384, input.ReadVarInt());
        Assert.Equal(2097151, input.ReadVarInt());
        Assert.Equal(2097152, input.ReadVarInt());
        Assert.Equal(134217728, input.ReadVarInt());
        Assert.Equal(268435455, input.ReadVarInt());
        Assert.Equal(8192, input.ReadVarInt());
        Assert.Equal(13371337, input.ReadVarInt());
    }
}