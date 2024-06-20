using Shockky.Lingo.Enums;

namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Statement;

public class PutStmtNode : StmtNode
{
    public PutStmtNode(PutType type, Node var, Node val) : base(NodeType.kPutStmtNode)
    {
        Type = type;
        Var = var;
        Var.Parent = this;
        Val = val;
        Val.Parent = this;
    }

    public PutType Type { get; }
    public Node Var { get; }
    public Node Val { get; }
}