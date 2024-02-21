﻿using Shockky.IO;

namespace Shockky.Resources.Cast;

public class XtraCastProperties : IMemberProperties
{
    public string SymbolName { get; set; }
    public byte[] Data { get; set; }

    public XtraCastProperties(ref ShockwaveReader input, ReaderContext context)
    {
        SymbolName = input.ReadString(input.ReadInt32());
        Data = new byte[input.ReadInt32()];
        input.ReadBytes(Data);
    }

    public int GetBodySize(WriterOptions options)
    {
        int size = 0;
        size += sizeof(int);
        size += SymbolName.Length;
        size += sizeof(int);
        size += Data.Length;
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        throw new NotImplementedException(nameof(XtraCastProperties));
        output.Write(SymbolName.Length);
        output.Write(Data.Length);
        output.Write(Data);
    }
}