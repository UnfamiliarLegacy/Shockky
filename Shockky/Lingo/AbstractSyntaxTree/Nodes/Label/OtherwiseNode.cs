namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Label;

public class OtherwiseNode : LabelNode
{
    public OtherwiseNode() : base(NodeType.kOtherwiseNode)
    {
        Block = new BlockNode();
        Block.Parent = this;
    }
    
    public BlockNode Block { get; }
}