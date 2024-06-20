using Shockky.Lingo.Enums;

namespace Shockky.Lingo.AbstractSyntaxTree.Nodes;

public class CaseLabelNode : LabelNode
{
    public CaseLabelNode(Node value, CaseExpect expect) : base(NodeType.kCaseLabelNode)
    {
        Value = value;
        Expect = expect;
        Value.Parent = this;
    }
    
    public Node Value { get; }
    public CaseExpect Expect { get; }
    
    public CaseLabelNode? NextOr { get; set; }
    public CaseLabelNode? NextLabel { get; set; }
    public BlockNode? Block { get; set; }
}