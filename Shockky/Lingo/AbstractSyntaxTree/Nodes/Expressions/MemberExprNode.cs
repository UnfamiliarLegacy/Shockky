namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Expressions;

public class MemberExprNode : ExprNode
{
    public MemberExprNode(string type, Node memberId, Node? castId) : base(NodeType.kMemberExprNode)
    {
        Type = type;
        MemberId = memberId;
        MemberId.Parent = this;

        if (castId != null)
        {
            CastId = castId;
            CastId.Parent = this;
        }
    }
    
    public string Type { get; }
    public Node MemberId { get; }
    public Node? CastId { get; }
}