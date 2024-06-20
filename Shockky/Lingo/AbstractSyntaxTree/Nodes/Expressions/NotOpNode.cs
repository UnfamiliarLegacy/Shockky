namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Expressions;

public class NotOpNode : ExprNode
{
    public NotOpNode(Node operand) : base(NodeType.kNotOpNode)
    {
        Operand = operand;
        Operand.Parent = this;
    }

    public Node Operand { get; }
}