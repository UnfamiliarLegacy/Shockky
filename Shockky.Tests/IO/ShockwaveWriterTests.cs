using Shockky.IO;

using Xunit;

namespace Shockky.Tests.IO;

public class ShockwaveWriterTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(127)]
    [InlineData(128)]
    [InlineData(8192)]
    [InlineData(16383)]
    [InlineData(16384)]
    [InlineData(2097151)]
    [InlineData(2097152)]
    [InlineData(13371337)]
    [InlineData(134217728)]
    [InlineData(268435455)]
    public void Write_VariableInteger_AreEqual(int value)
    {
        Span<byte> buffer = stackalloc byte[ShockwaveWriter.GetVarIntSize(value)];

        var output = new ShockwaveWriter(buffer, isBigEndian: false);
        var input = new ShockwaveReader(buffer, isBigEndian: false);

        output.WriteVarInt(value);

        Assert.Equal(value, input.ReadVarInt());
        Assert.False(input.IsDataAvailable);
    }

    [Fact]
    public void Write_BigEndian_NumericValues_AreEqual() => Write_NumericValues_AreEqual(isBigEndian: true);

    [Fact]
    public void Write_LittleEndian_NumericValues_AreEqual() => Write_NumericValues_AreEqual(isBigEndian: false);

    private void Write_NumericValues_AreEqual(bool isBigEndian)
    {
        const int OUTPUT_SIZE = sizeof(byte)
            + sizeof(short) * 2
            + sizeof(ushort) * 2
            + sizeof(int) * 2
            + sizeof(uint) * 2;

        Span<byte> buffer = stackalloc byte[OUTPUT_SIZE];
        var output = new ShockwaveWriter(buffer, isBigEndian);
        var input = new ShockwaveReader(buffer, isBigEndian);

        output.Write((byte)42);
        output.Write((short)4242);
        output.WriteBE((short)4242);
        output.Write((ushort)4242);
        output.WriteBE((ushort)4242);
        output.Write(123456789);
        output.WriteBE(123456789);
        output.Write((uint)123456789);
        output.WriteBE((uint)123456789);

        Assert.Equal(42, input.ReadByte());
        Assert.Equal(4242, input.ReadInt16());
        Assert.Equal(4242, input.ReadBEInt16());
        Assert.Equal((ushort)4242, input.ReadUInt16());
        Assert.Equal((ushort)4242, input.ReadBEUInt16());
        Assert.Equal(123456789, input.ReadInt32());
        Assert.Equal(123456789, input.ReadBEInt32());
        Assert.Equal((uint)123456789, input.ReadUInt32());
        Assert.Equal((uint)123456789, input.ReadBEUInt32());
    }
}
