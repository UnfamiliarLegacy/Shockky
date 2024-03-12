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

    public LingoScript()
    { }
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

        int eventHandlerCount = input.ReadInt16LittleEndian();
        int eventHandlerIndexOffset = input.ReadInt32LittleEndian();

        EventFlags = (LingoEventFlags)input.ReadInt32LittleEndian();

        short propertiesCount = input.ReadInt16LittleEndian();
        int propertiesOffset = input.ReadInt32LittleEndian();

        short globalsCount = input.ReadInt16LittleEndian();
        int globalsOffset = input.ReadInt32LittleEndian();

        short functionsCount = input.ReadInt16LittleEndian();
        int functionsOffset = input.ReadInt32LittleEndian();

        short literalsCount = input.ReadInt16LittleEndian();
        int literalsOffset = input.ReadInt32LittleEndian();

        int literalDataLength = input.ReadInt32LittleEndian();
        int literalDataOffset = input.ReadInt32LittleEndian();
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
