namespace Pdcl.Core.Syntax;

internal interface IVisitor<TNode> where TNode : SyntaxNode
{
    Task<TNode> VisitAsync(Parser parser);
}