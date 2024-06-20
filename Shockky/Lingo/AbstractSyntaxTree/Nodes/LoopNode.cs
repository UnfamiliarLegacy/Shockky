namespace Shockky.Lingo.AbstractSyntaxTree.Nodes;

public class LoopNode : StmtNode
{
    public LoopNode(NodeType type, int startIndex) : base(type)
    {
        StartIndex = startIndex;
        IsLoop = true;
    }
    
    public int StartIndex { get; }
}