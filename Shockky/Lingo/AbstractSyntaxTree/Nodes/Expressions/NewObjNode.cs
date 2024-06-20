namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Expressions;

public class NewObjNode : ExprNode
{
    public NewObjNode(string objType, Node args) : base(NodeType.kNewObjNode)
    {
        ObjType = objType;
        Args = args;
    }
    
    public string ObjType { get; }
    public Node Args { get; }
}