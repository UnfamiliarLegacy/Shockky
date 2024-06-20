namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Statement;

public class PlayCmdStmtNode : StmtNode
{
    public PlayCmdStmtNode(Node argList) : base(NodeType.kPlayCmdStmtNode)
    {
        ArgList = argList;
        ArgList.Parent = this;
    }

    public Node ArgList { get; }
}