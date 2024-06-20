namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Statement;

public class SoundCmdStmtNode : StmtNode
{
    public SoundCmdStmtNode(string cmd, Node argList) : base(NodeType.kSoundCmdStmtNode)
    {
        Cmd = cmd;
        ArgList = argList;
        ArgList.Parent = this;
    }

    public string Cmd { get; }
    public Node ArgList { get; }
}