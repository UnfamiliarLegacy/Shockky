namespace Shockky.Lingo.AbstractSyntaxTree.Nodes;

public class CommentNode(string text) : Node(NodeType.kCommentNode)
{
    public string Text { get; } = text;
}