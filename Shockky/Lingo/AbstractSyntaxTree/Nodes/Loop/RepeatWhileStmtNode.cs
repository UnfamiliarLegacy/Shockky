namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Loop;

public class RepeatWhileStmtNode : LoopNode
{
    public RepeatWhileStmtNode(int startIndex, Node condition) : base(NodeType.kRepeatWhileStmtNode, startIndex)
    {
        Condition = condition;
        Condition.Parent = this;
        Block = new BlockNode();
        Block.Parent = this;
    }
    
    public Node Condition { get; }
    public BlockNode Block { get; }
}