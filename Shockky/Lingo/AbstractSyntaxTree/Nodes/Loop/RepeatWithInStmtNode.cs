namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Loop;

public class RepeatWithInStmtNode : LoopNode
{
    public RepeatWithInStmtNode(int startIndex, string varName, Node list) : base(NodeType.kRepeatWithInStmtNode, startIndex)
    {
        VarName = varName;
        List = list;
        List.Parent = this;
        Block = new BlockNode();
        Block.Parent = this;
    }
    
    public string VarName { get; }
    public Node List { get; }
    public BlockNode Block { get; }
}