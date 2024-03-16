using System.Diagnostics.Contracts;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using Pdcl.Core.Syntax.Semantics;

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

        BinaryOperator operator_ = new BinaryOperator(op!.Value);
        parser.tokens.Increment();
        ValueNode? right = await VisitorFactory.GetVisitorFor<ValueNode>(parser.context)!.VisitAsync(parser);
        
        TypeNode? resultType = right == null ? null :
            left.Type.BinaryExpTypeCheck(right.Type, operator_.OperatorType) ?? right.Type.BinaryExpTypeCheck(left.Type, operator_.OperatorType);
        if (resultType == null) 
        {
            await parser.diagnostics.ReportTypeCheck(parser.tokens.Current.Metadata.Line);
        }
        return new BinaryExpression(operator_, left, right!, resultType!);
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
        if (!SyntaxHelper.CheckTokens(parser, SyntaxKind.OpenParentheseToken))
            return null;
        
        parser.tokens.Increment();

        ValueNode? value = await VisitorFactory.GetVisitorFor<ValueNode>(parser.context)!.VisitAsync(parser);
        if (parser.tokens.Current.Kind != SyntaxKind.CloseParentheseToken)
        {
            await parser.diagnostics.ReportUnsuitableSyntaxToken(parser.tokens.Current.Metadata.Line,
                parser.tokens.Current, SyntaxKind.CloseParentheseToken);
        }
        else parser.tokens.Increment();
        return new ParenthesizedExpression(value!);
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
        ValueNode? right = await VisitorFactory.GetVisitorFor<ValueNode>(parser.context)!.VisitAsync(parser);

        return new AssignExpression((RefValueNode)left, right!);
    }
    public Task<AssignExpression?> VisitAsync(Parser parser)
    {
        throw new NotImplementedException("Use overloaded method instead.");
    }

}
internal sealed class UnaryExpressionVisitor : IExpressionVisitor<UnaryExpression>
{
    public static UnaryExpressionVisitor Instance => _instance;
    private static UnaryExpressionVisitor _instance = new();
    private UnaryExpressionVisitor() { }
    /// <summary>
    /// Postfix unary operation (left value has been already parsed)
    /// </summary>
    /// <param name="parser"></param>
    /// <param name="left"></param>
    /// <returns></returns>
    public async Task<UnaryExpression?> VisitAsync(Parser parser, ValueNode left)
    {
        if (!SyntaxFacts.IsUnaryOperator(parser.tokens.Current.Kind, out UnaryOperator.UnaryOperators? op))
        {
            await parser.diagnostics.ReportUnkownOperationSyntax(parser.tokens.Current.Metadata.Line,
                parser.tokens.Current.Metadata.Raw);
            return null;
        }

        parser.tokens.Increment();
        UnaryOperator postfixOp = new UnaryOperator(op!.Value, false);
        TypeNode? resultType = left.Type.UnaryTypeCheck(postfixOp.OperatorType);
        if (resultType == null) 
        {
            await parser.diagnostics.ReportTypeCheck(parser.tokens.Current.Metadata.Line);
        }
        return new UnaryExpression(postfixOp, left, resultType!);
    }
    /// <summary>
    /// Prefix unary operation
    /// </summary>
    /// <param name="parser"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<UnaryExpression?> VisitAsync(Parser parser)
    {
        if (!SyntaxFacts.IsUnaryOperator(parser.tokens.Current.Kind, out UnaryOperator.UnaryOperators? op))
        {
            await parser.diagnostics.ReportUnkownOperationSyntax(parser.tokens.Current.Metadata.Line,
                parser.tokens.Current.Metadata.Raw);
            return null;
        }

        parser.tokens.Increment();
        TypeNode? castTypeNode = null; 
        if (op == UnaryOperator.UnaryOperators.Cast) 
        {
            castTypeNode = await VisitorFactory.GetVisitorFor<TypeNode>(parser.context)!.VisitAsync(parser);
            if (castTypeNode == null || parser.tokens.Current.Kind != SyntaxKind.CloseBracketToken) 
            {
                await parser.diagnostics.ReportUnsuitableSyntaxToken(parser.tokens.Current.Metadata.Line, 
                    parser.tokens.Current, SyntaxKind.CloseBracketToken);
                return null;
            }
            parser.tokens.Increment();
        }

        ValueNode? node = await ((ValueNodeVisitor)VisitorFactory.GetVisitorFor<ValueNode>(parser.context)!).VisitAsync(
            parser, checkPrefixUnary: castTypeNode != null, checkOtherExp: false);

        UnaryOperator prefixOp = new UnaryOperator(op!.Value, true);
        TypeNode? resultType = node?.Type.UnaryTypeCheck(op.Value);

        if (castTypeNode != null) 
            resultType = node!.Type.ExplicitTypeCheck(castTypeNode) ? castTypeNode : null;

        if (resultType == null) 
        {
            await parser.diagnostics.ReportTypeCheck(parser.tokens.Current.Metadata.Line);
        }
        return castTypeNode == null 
        ? new UnaryExpression(prefixOp, node!, resultType!)
        : new CastExpression(node!, castTypeNode!);
    }
}