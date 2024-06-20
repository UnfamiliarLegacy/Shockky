using Shockky.Lingo.Enums;

namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Expressions;

public class ChunkExprNode : ExprNode
{
    public ChunkExprNode(ChunkExprType type, Node first, Node last, Node s) : base(NodeType.kChunkExprNode)
    {
        Type = type;
        First = first;
        First.Parent = this;
        Last = last;
        Last.Parent = this;
        String = s;
        String.Parent = this;
    }
    
    public ChunkExprType Type { get; }
    public Node First { get; }
    public Node Last { get; }
    public Node String { get; }
}