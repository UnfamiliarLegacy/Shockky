namespace Shockky.Lingo.AbstractSyntaxTree.Nodes;

public class ExprNode : Node
{
    protected ExprNode(NodeType type) : base(type)
    {
        IsExpression = true;
    }
}