namespace Pdcl.Core.Syntax;

internal interface IExpressionVisitor<TExp> : IVisitor<TExp> where TExp : ExpressionNode 
{
    Task<TExp?> VisitAsync(Parser parser, ValueNode left);

}