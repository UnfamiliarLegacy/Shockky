namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Statement;

public class ChunkHiliteStmtNode : StmtNode
{
    public ChunkHiliteStmtNode(Node chunk) : base(NodeType.kChunkHiliteStmtNode)
    {
        Chunk = chunk;
        Chunk.Parent = this;
    }

    public Node Chunk { get; }
}