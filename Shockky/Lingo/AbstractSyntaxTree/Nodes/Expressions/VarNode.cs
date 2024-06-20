namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Expressions;

public class VarNode(string varName) : ExprNode(NodeType.kVarNode)
{
    public string VarName { get; } = varName;

    public override string ToString()
    {
        return $"{nameof(VarName)}: {VarName}";
    }
}