using Shockky.IO;
using Shockky.Resources.Cast;

namespace Shockky.Resources;

/// <summary>
/// Represents list of all cast members in the movie, sorted by the order they appear in the Score.
/// </summary>
public sealed class ScoreOrder : IShockwaveItem, IResource
{
    public OsType Kind => OsType.Sord;

    public CastMemberId[] Entries { get; set; }

    public ScoreOrder()
    { }
    public ScoreOrder(ref ShockwaveReader input, ReaderContext context)
    {
        input.ReverseEndianness = true;

        input.ReadInt32LittleEndian();
        input.ReadInt32LittleEndian();

        Entries = new CastMemberId[input.ReadInt32LittleEndian()];
        input.ReadInt32LittleEndian();

        input.ReadInt16LittleEndian();
        input.ReadInt16LittleEndian(); //TODO: dir <= 0x500 ? sizeof(short) : sizeof(short) * 2.

        for (int i = 0; i < Entries.Length; i++)
        {
            Entries[i] = new(input.ReadInt16LittleEndian(), input.ReadInt16LittleEndian());
        }
    }

    public int GetBodySize(WriterOptions options)
    {
        int size = 0;
        size += sizeof(int);
        size += sizeof(int);

        size += sizeof(int);

        size += sizeof(short);
        size += sizeof(short);

        size += sizeof(short) * 2 * Entries.Length;
        return size;
    }
    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        const short ENTRIES_OFFSET = 20;
        const short ENTRY_SIZE = sizeof(short) + sizeof(short);

        output.WriteInt32LittleEndian(0);
        output.WriteInt32LittleEndian(0);

        output.WriteInt32LittleEndian(Entries.Length);
        output.WriteInt32LittleEndian(Entries.Length);

        output.WriteInt16LittleEndian(ENTRIES_OFFSET);
        output.WriteInt16LittleEndian(ENTRY_SIZE);

        foreach (CastMemberId memberId in Entries)
        {
            output.WriteMemberId(memberId);
        }
    }
}
