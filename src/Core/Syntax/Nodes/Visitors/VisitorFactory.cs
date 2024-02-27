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
            case UseNode:
                return (IVisitor<TNode>)(IVisitor<UseNode>)UseVisitor.Instance;
            case ConstVarNode:
                return (IVisitor<TNode>)(IVisitor<ConstVarNode>)ConstVarVisitor.Instance; 
            default:
                return null;
        }

    }
}