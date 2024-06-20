using Shockky.Lingo.Enums;

namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Expressions;

public class BinaryOpNode : ExprNode
{
    public BinaryOpNode(LingoOpCode opCode, Node left, Node right) : base(NodeType.kBinaryOpNode)
    {
        OpCode = opCode;
        Left = left;
        Left.Parent = this;
        Right = right;
        Right.Parent = this;
    }
    
    public LingoOpCode OpCode { get; }
    public Node Left { get; }
    public Node Right { get; }
}
