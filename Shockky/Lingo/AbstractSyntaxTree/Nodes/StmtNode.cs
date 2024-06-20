namespace Shockky.Lingo.AbstractSyntaxTree.Nodes;

public class StmtNode : Node
{
    protected StmtNode(NodeType type) : base(type)
    {
        IsStatement = true;
    }
}