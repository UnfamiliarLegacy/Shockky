using System.Drawing;

using Shockky.IO;

namespace Shockky.Resources.Cast;

public sealed class BitmapCastProperties : IMemberProperties
{
    public ushort Stride { get; set; }
    public Rectangle Rectangle { get; set; }
    public byte AlphaThreshold { get; set; }
    public byte[] OLE { get; set; }
    public Point RegistrationPoint { get; set; }
    public BitmapFlags Flags { get; set; }

    public byte BitDepth { get; set; } = 1;
    public CastMemberId PaletteRef { get; set; } = new CastMemberId(-1, 0);

    public BitmapCastProperties(ref ShockwaveReader input, ReaderContext context)
    {
        Stride = input.ReadUInt16();
        Rectangle = input.ReadRect();
        //TODO: if version < 0x700 ? i16 : i8 i8
        AlphaThreshold = input.ReadByte();
        input.ReadByte();
        OLE = input.ReadBytes(6).ToArray();
        RegistrationPoint = input.ReadPoint();
        Flags = (BitmapFlags)input.ReadByte();

        if ((Stride & 0x8000u) != 0)
        {
            Stride &= 0x3FFF;

            BitDepth = input.ReadByte(); //if size == 26: i16 to u8 else u8 - src: eq-rs
            PaletteRef = new CastMemberId(input.ReadInt16(), input.ReadInt16()); //is_v5 ? new CastMemberId(input.ReadInt16(), input.ReadInt16()); : new CastMemberId(-1, input.ReadInt16());
        }
    }

    public int GetBodySize(WriterOptions options)
    {
        int size = 0;
        size += sizeof(short);
        size += sizeof(short) * 4;
        size += sizeof(byte);
        size += sizeof(byte);
        size += 6;
        size += sizeof(short);
        size += sizeof(short);
        size += sizeof(byte);

        if (BitDepth != 1)
        {
            size += sizeof(byte);
            size += sizeof(int);
        }
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        if (BitDepth != 1)
            Stride |= 0x8000;

        output.Write(Stride); 

        output.Write(Rectangle);
        output.Write(AlphaThreshold);
        output.Write((byte)0);
        output.Write(OLE);
        output.Write(RegistrationPoint);
        output.Write((byte)Flags);

        if (BitDepth == 1) return;
        output.Write(BitDepth);
        output.Write(PaletteRef);
    }
}
