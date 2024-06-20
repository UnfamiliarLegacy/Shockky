namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Expressions;

public class ObjBracketExprNode : ExprNode
{
    public ObjBracketExprNode(Node obj, Node prop) : base(NodeType.kObjBracketExprNode)
    {
        Obj = obj;
        Obj.Parent = this;
        Prop = prop;
        Prop.Parent = this;
    }

    public Node Obj { get; }
    public Node Prop { get; }
}