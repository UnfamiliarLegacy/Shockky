using Shockky.Lingo.AbstractSyntaxTree.Nodes;

namespace Shockky.Lingo.AbstractSyntaxTree;

public class AST
{
    public AST(LingoHandler handler)
    {
        Root = new HandlerNode(handler);
        CurrentBlock = Root.Block;
    }

    public HandlerNode Root { get; }
    public BlockNode? CurrentBlock { get; private set; }

    public void AddStatement(Node statement)
    {
        CurrentBlock!.AddChild(statement);
    }

    public void EnterBlock(BlockNode node)
    {
        CurrentBlock = node;
    }

    public void ExitBlock()
    {
        var ancenstorStatement = CurrentBlock?.AncestorStatement();
        if (ancenstorStatement == null)
        {
            CurrentBlock = null;
            return;
        }

        var block = ancenstorStatement.Parent;
        if (block == null || block.Type != NodeType.kBlockNode)
        {
            CurrentBlock = null;
            return;
        }

        CurrentBlock = (BlockNode)block;
    }
}