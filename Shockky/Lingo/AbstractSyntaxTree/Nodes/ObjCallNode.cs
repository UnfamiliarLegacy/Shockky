namespace Shockky.Lingo.AbstractSyntaxTree.Nodes;

public class ObjCallNode : Node
{
    public ObjCallNode(string name, Node argList) : base(NodeType.kObjCallNode)
    {
        Name = name;
        ArgList = argList;
        ArgList.Parent = this;
        
        if (ArgList.GetValue().Type == DatumType.kDatumArgListNoRet)
        {
            IsStatement = true;
        }
        else
        {
            IsExpression = true;
        }
    }
    
    public string Name { get; }
    public Node ArgList { get; }

    public override string ToString()
    {
        return $"{nameof(Name)}: {Name}, {nameof(ArgList)}: [{ArgList}]";
    }
}