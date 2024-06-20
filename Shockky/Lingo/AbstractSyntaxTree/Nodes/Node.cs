namespace Shockky.Lingo.AbstractSyntaxTree.Nodes;

public class Node(NodeType type)
{
    public NodeType Type { get; } = type;
    public bool IsExpression { get; protected set; }
    public bool IsStatement { get; protected set; }
    public bool IsLabel { get; protected set; }
    public bool IsLoop { get; protected set; }
    public Node? Parent { get; protected internal set; }

    public virtual Datum GetValue()
    {
        return new Datum();
    }

    public Node? AncestorStatement()
    {
        var ancestor = Parent;

        while (ancestor != null && !ancestor.IsStatement)
        {
            ancestor = ancestor.Parent;
        }

        return ancestor;
    }

    public LoopNode? AncestorLoop()
    {
        var ancestor = Parent;

        while (ancestor != null && !ancestor.IsLoop)
        {
            ancestor = ancestor.Parent;
        }

        return (LoopNode?)ancestor;
    }
}