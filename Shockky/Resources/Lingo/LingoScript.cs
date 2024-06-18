using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using Shockky.IO;
using Shockky.Lingo;

namespace Shockky.Resources;

public sealed class LingoScript : IShockwaveItem, IResource
{
    public OsType Kind => OsType.Lscr;

    /// <summary>
    /// The index of the script within its owner <see cref="LingoContext">Context</see>.
    /// </summary>
    public short ContextIndex { get; set; }

    /// <summary>
    /// The index of the <see cref="LingoEnvironment">Environment</see> that owns the <see cref="LingoContext">Context</see> that owns this script.
    /// </summary>
    public short EnvironmentIndex { get; set; }

    /// <summary>
    /// For factory scripts, the index of the parent script within its owner <see cref="LingoContext"/>
    /// </summary>
    public short ParentContextIndex { get; set; }

    public short CastMemberId { get; set; }

    public LingoScriptFlags Flags { get; set; }
    public short FactoryNameIndex { get; set; }

    /// <summary>
    /// Represents all the events that this script handles.
    /// </summary>
    public LingoEventFlags EventFlags { get; set; }

    public List<short> EventHandlerIndices { get; set; }
    public List<short> Properties { get; set; }
    public List<short> Globals { get; set; }

    /// <summary>
    /// Represents all lingo handlers in this script.
    /// </summary>
    public List<LingoFunction> Functions { get; set; }

    public List<LingoLiteral> Literals { get; }

    public LingoScript()
    {
        EventHandlerIndices = new List<short>();
        Properties = new List<short>();
        Globals = new List<short>();
        Functions = new List<LingoFunction>();
        Literals = new List<LingoLiteral>();
    }
    public LingoScript(ref ShockwaveReader input)
    {
        input.ReverseEndianness = true;

        input.ReadInt32LittleEndian();
        input.ReadInt32LittleEndian();

        input.ReadInt32LittleEndian();
        input.ReadInt32LittleEndian();

        input.ReadInt16LittleEndian();

        ContextIndex = input.ReadInt16LittleEndian();
        EnvironmentIndex = input.ReadInt16LittleEndian();
        ParentContextIndex = input.ReadInt16LittleEndian();
        short environmentFactoryIndexGarbageDebug = input.ReadInt16LittleEndian();

        input.ReadInt32LittleEndian();
        input.ReadInt32LittleEndian();
        input.ReadInt32LittleEndian();

        Flags = (LingoScriptFlags)input.ReadInt32LittleEndian();

        input.ReadInt32LittleEndian();
        CastMemberId = input.ReadInt16LittleEndian();

        FactoryNameIndex = input.ReadInt16LittleEndian();

        EventHandlerIndices = new List<short>(input.ReadInt16LittleEndian());
        int eventHandlerIndexOffset = input.ReadInt32LittleEndian();

        EventFlags = (LingoEventFlags)input.ReadInt32LittleEndian();

        Properties = new List<short>(input.ReadInt16LittleEndian());
        int propertiesOffset = input.ReadInt32LittleEndian();

        Globals = new List<short>(input.ReadInt16LittleEndian());
        int globalsOffset = input.ReadInt32LittleEndian();

        Functions = new List<LingoFunction>(input.ReadInt16LittleEndian());
        int functionsOffset = input.ReadInt32LittleEndian();

        Literals = new List<LingoLiteral>(input.ReadInt16LittleEndian());
        int literalsOffset = input.ReadInt32LittleEndian();

        int literalDataLength = input.ReadInt32LittleEndian();
        int literalDataOffset = input.ReadInt32LittleEndian();

        input.Position = propertiesOffset;
        for (int i = 0; i < Properties.Capacity; i++)
        {
            Properties.Add(input.ReadInt16LittleEndian());
        }

        input.Position = globalsOffset;
        for (int i = 0; i < Globals.Capacity; i++)
        {
            Globals.Add(input.ReadInt16LittleEndian());
        }

        input.Position = functionsOffset;
        for (int i = 0; i < Functions.Capacity; i++)
        {
            Functions.Add(new LingoFunction(ref input));
        }

        input.Position = literalsOffset;
        var literalEntries = new (VariantKind Kind, int Offset)[Literals.Capacity]; // TODO: Stackalloc/ArrayPool
        for (int i = 0; i < Literals.Capacity; i++)
        {
            literalEntries[i].Kind = (VariantKind)input.ReadInt32LittleEndian();
            literalEntries[i].Offset = input.ReadInt32LittleEndian();
        }

        input.Position = literalDataOffset;
        for (int i = 0; i < Literals.Capacity; i++)
        {
            (VariantKind kind, int offset) = literalEntries[i];

            Literals.Add(LingoLiteral.Read(ref input, kind, literalDataOffset + offset));

            Debug.Assert(literalDataOffset + literalDataLength >= input.Position);
        }

        input.Position = eventHandlerIndexOffset;
        for (int i = 0; i < EventHandlerIndices.Capacity; i++)
        {
            EventHandlerIndices.Add(input.ReadInt16LittleEndian());
        }
    }

    public static int GetHeaderSize()
    {
        int size = 0;
        size += sizeof(int);
        size += sizeof(int);

        size += sizeof(int);
        size += sizeof(int);

        size += sizeof(short);
        size += sizeof(short);
        size += sizeof(short);
        size += sizeof(short);
        size += sizeof(short);

        size += sizeof(int);
        size += sizeof(int);
        size += sizeof(int);

        size += sizeof(int);

        size += sizeof(int);
        size += sizeof(short);

        size += sizeof(short);

        size += sizeof(short);

        size += sizeof(int);
        size += sizeof(int);

        size += sizeof(short);
        size += sizeof(int);

        size += sizeof(short);
        size += sizeof(int);

        size += sizeof(short);
        size += sizeof(int);

        size += sizeof(int);
        size += sizeof(int);
        return size;
    }

    public int GetBodySize(WriterOptions options)
    {
        int size = 0;
        size += GetHeaderSize();
        return size;
    }

    public void WriteTo(ShockwaveWriter output, WriterOptions options)
    {
        output.WriteInt32LittleEndian(0);
        output.WriteInt32LittleEndian(0);

        int bodySize = GetBodySize(options);
        output.WriteInt32LittleEndian(bodySize);
        output.WriteInt32LittleEndian(bodySize);

        output.WriteInt32LittleEndian(92); //TODO:

        output.WriteInt16LittleEndian(ContextIndex);
        output.WriteInt16LittleEndian(EnvironmentIndex);
        output.WriteInt16LittleEndian(ParentContextIndex);
        output.WriteInt16LittleEndian((short)-1);

        output.WriteInt32LittleEndian(0);
        output.WriteInt32LittleEndian(0);
        output.WriteInt32LittleEndian(0);

        output.WriteInt32LittleEndian((int)Flags);

        output.WriteInt16LittleEndian((short)0);
        output.WriteInt16LittleEndian(CastMemberId);

        output.WriteInt16LittleEndian(FactoryNameIndex);
    }
}
