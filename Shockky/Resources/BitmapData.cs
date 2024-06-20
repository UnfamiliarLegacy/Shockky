﻿using System.Buffers;

using Shockky.IO;
using Shockky.IO.Compression;
using Shockky.Resources.Cast;
using Shockky.Resources.Cast.Properties;
using Shockky.Resources.Enum;

namespace Shockky.Resources;

public sealed partial class BitmapData : IShockwaveItem, IResource
{
    public OsType Kind => OsType.BITD;

    public byte[] Data { get; set; }

    public BitmapData(ref ShockwaveReader input, ReaderContext context)
    {
        Data = new byte[input.Length];
        input.ReadBytes(Data);
    }

    public bool TryDecompress(BitmapCastProperties properties, Span<byte> output, out int bytesWritten)
    {
        int outputLength = properties.Stride * properties.Rectangle.Height;

        bytesWritten = 0;
        if (outputLength == 0)
            return false;

        if (Data.Length == outputLength)
        {
            Data.CopyTo(output);
            bytesWritten = Data.Length;
            return true;
        }
        return RLE.TryDecompress(Data, output, out bytesWritten);
    }

    public int GetBodySize(WriterOptions options) => Data.Length;
    public void WriteTo(ShockwaveWriter output, WriterOptions options) => output.WriteBytes(Data);
}
