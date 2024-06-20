namespace Shockky.Lingo.AbstractSyntaxTree.Nodes;

public class HandlerNode : Node
{
    public HandlerNode(LingoHandler handler) : base(NodeType.kHandlerNode)
    {
        Handler = handler;
        Block = new BlockNode();
        Block.Parent = this;
    }
    
    public LingoHandler Handler { get; }
    public BlockNode Block { get; set; }
}