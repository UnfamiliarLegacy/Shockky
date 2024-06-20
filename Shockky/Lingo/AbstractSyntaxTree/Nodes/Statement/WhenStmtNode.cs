namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Statement;

public class WhenStmtNode(int _event, string script) : StmtNode(NodeType.kWhenStmtNode)
{
    public int Event { get; } = _event;
    public string Script { get; } = script;
}