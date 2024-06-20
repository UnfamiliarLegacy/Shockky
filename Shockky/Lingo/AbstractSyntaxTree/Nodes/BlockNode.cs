namespace Shockky.Lingo.AbstractSyntaxTree.Nodes;

public class BlockNode : Node
{
    public BlockNode() : base(NodeType.kBlockNode)
    {
    }

    public uint? EndPos { get; set; }
    public CaseLabelNode? CurrentCaseLabel { get; set; }
    public List<Node> Children { get; } = [];

    public void AddChild(Node child)
    {
        child.Parent = this;
        Children.Add(child);
    }
}