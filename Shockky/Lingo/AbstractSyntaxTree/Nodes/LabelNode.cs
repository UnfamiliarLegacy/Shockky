namespace Shockky.Lingo.AbstractSyntaxTree.Nodes;

public class LabelNode : Node
{
    public LabelNode(NodeType type) : base(type)
    {
        IsLabel = true;
    }
}