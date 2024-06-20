using Shockky.Lingo.AbstractSyntaxTree.Nodes.Label;

namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Statement;

public class CaseStmtNode : StmtNode
{
    public CaseStmtNode(Node value) : base(NodeType.kCaseStmtNode)
    {
        Value = value;
        Value.Parent = this;
    }
    
    public Node Value { get; }
    public CaseLabelNode? FirstLabel { get; set; }
    public OtherwiseNode? Otherwise { get; set; }

    public uint EndPos { get; set; }
    public uint PotentialOtherwisePos { get; set; }

    public void AddOtherwise()
    {
        Otherwise = new OtherwiseNode();
        Otherwise.Parent = this;
        Otherwise.Block.EndPos = EndPos;
    }
}