namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Statement;

public class AssignmentStmtNode : StmtNode
{
    public AssignmentStmtNode(Node variable, Node value, bool forceVerbose = false) : base(NodeType.kAssignmentStmtNode)
    {
        Variable = variable;
        Variable.Parent = this;
        Value = value;
        Value.Parent = this;
        ForceVerbose = forceVerbose;
    }

    public Node Variable { get; }
    public Node Value { get; }
    public bool ForceVerbose { get; }
}