namespace Pdcl.Core.Syntax;

internal static class VisitorFactory
{
    public static IVisitor<TNode>? GetVisitorFor<TNode>() where TNode : SyntaxNode 
    {
        TNode? default_ = default;
        switch(default_) 
        {
            case SyntaxTree.ApplicationContextNode:
                return (IVisitor<TNode>)(IVisitor<SyntaxTree.ApplicationContextNode>)ApplicationContextVisitor.Instance;
            default:
                return null;
        }

    }
}