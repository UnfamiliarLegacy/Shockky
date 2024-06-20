namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Statement;

public class IfStmtNode : StmtNode
{
    public IfStmtNode(Node condition) : base(NodeType.kIfStmtNode)
    {
        Condition = condition;
        Condition.Parent = this;
        Block1 = new BlockNode();
        Block1.Parent = this;
        Block2 = new BlockNode();
        Block2.Parent = this;
    }
    
    public Node Condition { get; }
    public BlockNode Block1 { get; }
    public BlockNode Block2 { get; }
    public bool HasElse { get; set; }
}