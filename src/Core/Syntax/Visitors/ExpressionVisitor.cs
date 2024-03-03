using System.Diagnostics.Contracts;

namespace Pdcl.Core.Syntax;

internal sealed class BinaryExpressionVisitor : IExpressionVisitor<BinaryExpression>
{
    public static BinaryExpressionVisitor Instance => _instance;
    private static BinaryExpressionVisitor _instance = new();
    private BinaryExpressionVisitor() { }
    public async Task<BinaryExpression?> VisitAsync(Parser parser, ValueNode left)
    {
        if (!SyntaxFacts.IsBinaryOperator(parser.tokens.Current.Kind, out BinaryOperator.BinaryOperators? op))
        {
            await parser.diagnostics.ReportUnkownOperationSyntax(parser.tokens.Current.Metadata.Line,
                parser.tokens.Current.Metadata.Raw);
            return null;
        }
        BinaryOperator operator_ = new BinaryOperator(op!.Value, parser.tokens.Index);
        parser.tokens.Increment();
        ValueNode? right = await VisitorFactory.GetVisitorFor<ValueNode>()!.VisitAsync(parser);
        return new BinaryExpression(operator_, left, right!, parser.tokens.Index);
    }
    public Task<BinaryExpression?> VisitAsync(Parser parser)
    {
        throw new NotImplementedException("Use overloaded method instead.");
    }
}
internal sealed class ParenthesizedExpressionVisitor : IExpressionVisitor<ParenthesizedExpression>
{
    public static ParenthesizedExpressionVisitor Instance => _intance;
    private static ParenthesizedExpressionVisitor _intance = new();
    private ParenthesizedExpressionVisitor() { }

    public async Task<ParenthesizedExpression?> VisitAsync(Parser parser)
    {
        if (parser.tokens.Current.Kind != SyntaxKind.OpenParentheseToken)
        {
            await parser.diagnostics.ReportUnsuitableSyntaxToken(parser.tokens.Current.Metadata.Line,
                parser.tokens.Current, SyntaxKind.OpenParentheseToken);
            return null;
        }
        else parser.tokens.Increment();

        ValueNode? value = await VisitorFactory.GetVisitorFor<ValueNode>()!.VisitAsync(parser);
        if (parser.tokens.Current.Kind != SyntaxKind.CloseParentheseToken)
        {
            await parser.diagnostics.ReportUnsuitableSyntaxToken(parser.tokens.Current.Metadata.Line,
                parser.tokens.Current, SyntaxKind.CloseParentheseToken);
        }
        else parser.tokens.Increment();
        return new ParenthesizedExpression(value!, parser.tokens.Index);
    }
    public Task<ParenthesizedExpression?> VisitAsync(Parser parser, ValueNode left)
        => VisitAsync(parser);
}
internal sealed class AssignExpressionVisitor : IExpressionVisitor<AssignExpression>
{
    public static AssignExpressionVisitor Instance => _instance;
    private static AssignExpressionVisitor _instance = new();
    private AssignExpressionVisitor() {}

    public async Task<AssignExpression?> VisitAsync(Parser parser, ValueNode left)
    {
        if (parser.tokens.Current.Kind != SyntaxKind.EqualToken)
        {
            await parser.diagnostics.ReportUnkownOperationSyntax(parser.tokens.Current.Metadata.Line,
                parser.tokens.Current.Metadata.Raw);
            return null;
        }
        parser.tokens.Increment();
        ValueNode? right = await VisitorFactory.GetVisitorFor<ValueNode>()!.VisitAsync(parser);

        return new AssignExpression((RefValueNode)left, right!, parser.tokens.Index);
    }
    public Task<AssignExpression?> VisitAsync(Parser parser)
    {
        throw new NotImplementedException("Use overloaded method instead.");
    }

}