using Shockky.Lingo.AbstractSyntaxTree.Nodes;
using Shockky.Lingo.Enums;

namespace Shockky.Lingo;

public class Bytecode
{
    public Bytecode(byte op, int obj, uint pos)
    {
        Op = op;
        OpCode = (LingoOpCode)(op >= 0x40 ? 0x40 + op % 0x40 : op);
        Obj = obj;
        Pos = pos;
        
        Tag = BytecodeTag.kTagNone;
        OwnerLoop = 0;
    }
    
    public byte Op { get; }
    public LingoOpCode OpCode { get; }
    public int Obj { get; }
    public uint Pos { get; }
    
    public BytecodeTag Tag { get; set; }
    public int? OwnerLoop { get; set; }
    public Node? Translation { get; set; }
}