﻿using Shockky.IO;

using System.Diagnostics;
using Shockky.Resources.Enum;

namespace Shockky.Resources;

[DebuggerDisplay("{Kind}")]
public readonly ref struct ResourceHeader
{
    public bool IsVariableLength => Kind switch
    {
        OsType.Fver or
        OsType.Fcdr or
        OsType.ABMP or
        OsType.FGEI => true,

        _ => false
    };

    public OsType Kind { get; }
    public int Length { get; }

    public ResourceHeader(OsType kind)
    {
        Kind = kind;
    }
    public ResourceHeader(ref ShockwaveReader input)
        : this((OsType)input.ReadInt32BigEndian())
    {
        Length = IsVariableLength ?
            input.Read7BitEncodedInt() : input.ReadInt32BigEndian();
    }

    public int GetBodySize(WriterOptions options)
    {
        int size = 0;
        size += sizeof(int);
        size += IsVariableLength ? ShockwaveWriter.GetVarIntSize(Length) : sizeof(int);
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        output.WriteInt32BigEndian((int)Kind);
        if (IsVariableLength)
        {
            output.Write7BitEncodedInt(Length);
        }
        else output.WriteInt32BigEndian(Length);
    }
}
