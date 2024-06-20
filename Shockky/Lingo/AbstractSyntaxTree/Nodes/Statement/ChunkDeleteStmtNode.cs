namespace Shockky.Lingo.AbstractSyntaxTree.Nodes.Statement;

public class ChunkDeleteStmtNode : StmtNode
{
    public ChunkDeleteStmtNode(Node chunk) : base(NodeType.kChunkDeleteStmtNode)
    {
        Chunk = chunk;
        Chunk.Parent = this;
    }

    public Node Chunk { get; }
}