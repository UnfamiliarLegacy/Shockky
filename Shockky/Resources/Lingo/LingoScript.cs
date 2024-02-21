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
        input.IsBigEndian = true;

        input.ReadInt32();
        input.ReadInt32();

        input.ReadInt32();
        input.ReadInt32();

        input.ReadInt16();

        ContextIndex = input.ReadInt16();
        EnvironmentIndex = input.ReadInt16();
        ParentContextIndex = input.ReadInt16();
        short environmentFactoryIndexGarbageDebug = input.ReadInt16();

        input.ReadInt32();
        input.ReadInt32();
        input.ReadInt32();

        Flags = (LingoScriptFlags)input.ReadInt32();

        input.ReadInt32();
        CastMemberId = input.ReadInt16();

        FactoryNameIndex = input.ReadInt16();

        int eventHandlerCount = input.ReadInt16();
        int eventHandlerIndexOffset = input.ReadInt32();

        EventFlags = (LingoEventFlags)input.ReadInt32();

        short propertiesCount = input.ReadInt16();
        int propertiesOffset = input.ReadInt32();

        short globalsCount = input.ReadInt16();
        int globalsOffset = input.ReadInt32();

        short functionsCount = input.ReadInt16();
        int functionsOffset = input.ReadInt32();

        short literalsCount = input.ReadInt16();
        int literalsOffset = input.ReadInt32();

        int literalDataLength = input.ReadInt32();
        int literalDataOffset = input.ReadInt32();
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
        output.Write(0);
        output.Write(0);

        int bodySize = GetBodySize(options);
        output.Write(bodySize);
        output.Write(bodySize);

        output.Write(92); //TODO:

        output.Write(ContextIndex);
        output.Write(EnvironmentIndex);
        output.Write(ParentContextIndex);
        output.Write((short)-1);

        output.Write(0);
        output.Write(0);
        output.Write(0);

        output.Write((int)Flags);

        output.Write((short)0);
        output.Write(CastMemberId);

        output.Write(FactoryNameIndex);
    }
}
