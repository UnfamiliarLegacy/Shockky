namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Loop;

public class RepeatWithToStmtNode : LoopNode
{
    public RepeatWithToStmtNode(int startIndex, string varName, Node start, bool up, Node end) : base(NodeType.kRepeatWithToStmtNode, startIndex)
    {
        VarName = varName;
        Start = start;
        Start.Parent = this;
        Up = up;
        End = end;
        End.Parent = this;
        Block = new BlockNode();
        Block.Parent = this;
    }
    
    public string VarName { get; }
    public Node Start { get; }
    public bool Up { get; }
    public Node End { get; }
    public BlockNode Block { get; }
}