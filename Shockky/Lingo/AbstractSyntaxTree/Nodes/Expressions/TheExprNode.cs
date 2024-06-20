namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Expressions;

public class TheExprNode(string prop) : ExprNode(NodeType.kTheExprNode)
{
    public string Prop { get; } = prop;
}