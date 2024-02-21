using Shockky.IO;

namespace Shockky.Resources;

public readonly record struct Guide(Axis Axis, short Position) : IShockwaveItem
{
    public static Guide Read(ref ShockwaveReader reader, ReaderContext context)
        => new((Axis)reader.ReadInt16(), reader.ReadInt16());

    public int GetBodySize(WriterOptions options)
    {
        int size = 0;
        size += sizeof(short);
        size += sizeof(short);
        return size;
    }
    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        output.Write((short)Axis);
        output.Write(Position);
    }
}
