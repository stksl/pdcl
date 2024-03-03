namespace Pdcl.Core.Syntax;

internal static class VisitorFactory
{
    public static IVisitor<TNode>? GetVisitorFor<TNode>() where TNode : SyntaxNode 
    {
        switch(typeof(TNode).Name) 
        {

            case nameof(SyntaxTree.ApplicationContextNode):
                return (IVisitor<TNode>)(IVisitor<SyntaxTree.ApplicationContextNode>)ApplicationContextVisitor.Instance;
            // top-level node visitor
            case nameof(UseNode):
                return (IVisitor<TNode>)(IVisitor<UseNode>)UseVisitor.Instance;
            case nameof(ConstVarNode):
                return (IVisitor<TNode>)(IVisitor<ConstVarNode>)ConstVarVisitor.Instance; 
                
                // expressions
            case nameof(BinaryExpression):
                return (IVisitor<TNode>)(IVisitor<BinaryExpression>)BinaryExpressionVisitor.Instance;
            case nameof(ParenthesizedExpression):
                return (IVisitor<TNode>)(IVisitor<ParenthesizedExpression>)ParenthesizedExpressionVisitor.Instance;
            case nameof(AssignExpression):
                return (IVisitor<TNode>)(IVisitor<AssignExpression>)AssignExpressionVisitor.Instance;

            case nameof(ValueNode):
                return (IVisitor<TNode>)(IVisitor<ValueNode>)ValueNodeVisitor.Instance;

            case nameof(TypeNode):
                return (IVisitor<TNode>)(IVisitor<TypeNode>)TypeNodeVisitor.Instance;
            default:
                return null;
        }

    }
}