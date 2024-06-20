using Shockky.Resources.Enum;
using Shockky.Resources.Lingo;

namespace Shockky.Lingo;

public class LingoDecompiler
{
    private readonly LingoNames _names;
    private readonly LingoScript _script;

    public LingoDecompiler(int version, LingoNames names, LingoScript script)
    {
        _names = names;
        _script = script;
        
        Version = version;
        Literals = _script.Literals;
        Handlers = _script.Functions
            .Select(x => new LingoHandler(this, x))
            .ToList();
        IsFactory = (_script.Flags & LingoScriptFlags.IsFactory) != 0;
    }

    public int Version { get; }
    public List<LingoLiteral> Literals { get; }
    public List<LingoHandler> Handlers { get; }
    public bool IsFactory { get; }

    internal bool IsValidName(int id)
    {
        return -1 < id && id < _names.Names.Count;
    }

    internal string GetName(int id)
    {
        return IsValidName(id) 
            ? _names.Names[id] 
            : $"UNKNOWN_NAME_{id}";
    }
    
    public void Disassemble()
    {
        if ((_script.Flags & LingoScriptFlags.Flag200_SCORE_RELATED) != 0 && _script.Functions.Capacity > 0)
        {
            Handlers[0].IsGenericEvent = true;
        }
        
        foreach (var dec in Handlers)
        {
            dec.ReadNames();
        }
        
        foreach (var dec in Handlers)
        {
            dec.Disassemble();
        }
    }
}