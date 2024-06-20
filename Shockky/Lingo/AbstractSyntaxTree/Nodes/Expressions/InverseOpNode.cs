namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Expressions;

public class InverseOpNode : ExprNode
{
    public InverseOpNode(Node operand) : base(NodeType.kInverseOpNode)
    {
        Operand = operand;
        Operand.Parent = this;
    }

    public Node Operand { get; }
}