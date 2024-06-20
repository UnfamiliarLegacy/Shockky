namespace Shockky.Lingo.AbstractSyntaxTree.Nodes;

public class CallNode : Node
{
    public CallNode(string name, Node argList) : base(NodeType.kCallNode)
    {
        Name = name;
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

    public string Name { get; }
    public Node ArgList { get; }
}