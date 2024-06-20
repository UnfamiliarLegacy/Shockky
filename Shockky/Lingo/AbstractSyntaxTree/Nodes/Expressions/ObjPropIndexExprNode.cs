namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Expressions;

public class ObjPropIndexExprNode : ExprNode
{
    public ObjPropIndexExprNode(Node obj, string prop, Node i, Node? index2) : base(NodeType.kObjPropIndexExprNode)
    {
        Obj = obj;
        Obj.Parent = this;
        Prop = prop;
        Index = i;
        Index.Parent = this;

        if (index2 != null)
        {
            Index2 = index2;
            Index2.Parent = this;
        }
    }

    public Node Obj { get; }
    public string Prop { get; }
    public Node Index { get; }
    public Node? Index2 { get; }
}