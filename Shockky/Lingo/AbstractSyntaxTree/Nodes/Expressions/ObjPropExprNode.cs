namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Expressions;

public class ObjPropExprNode : ExprNode
{
    public ObjPropExprNode(Node obj, string prop) : base(NodeType.kObjPropExprNode)
    {
        Obj = obj;
        Obj.Parent = this;
        Prop = prop;
    }

    public Node Obj { get; }
    public string Prop { get; }
}