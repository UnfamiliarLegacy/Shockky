namespace Shockky.Lingo.AbstractSyntaxTree.Nodes;

public class ObjCallV4Node : Node
{
    public ObjCallV4Node(Node obj, Node argList) : base(NodeType.kObjCallV4Node)
    {
        Obj = obj;
        ArgList = argList;
        ArgList.Parent = this;
        
        if (argList.GetValue().Type == DatumType.kDatumArgListNoRet)
        {
            IsStatement = true;
        }
        else
        {
            IsExpression = true;
        }
    }
    
    public Node Obj { get; }
    public Node ArgList { get; }
}