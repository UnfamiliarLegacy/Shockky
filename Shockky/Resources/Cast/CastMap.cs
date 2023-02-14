using Shockky.IO;

namespace Shockky.Resources;

public sealed class CastMap : IResource, IShockwaveItem
{
    public OsType Kind => OsType.CASPtr;

    public int[] Members { get; set; }

    public CastMap()
    { }
    public CastMap(ref ShockwaveReader input, ReaderContext context)
    {
        input.IsBigEndian = true;

        Members = new int[input.Length / sizeof(int)];
        for (int i = 0; i < Members.Length; i++)
        {
            Members[i] = input.ReadInt32();
        }
    }

    public int GetBodySize(WriterOptions options) => Members.Length * sizeof(int);

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        for (int i = 0; i < Members.Length; i++)
        {
            output.Write(Members[i]);
        }
    }
}
