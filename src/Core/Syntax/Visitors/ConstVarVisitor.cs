using Pdcl.Core.Syntax.Semantics;

namespace Pdcl.Core.Syntax;

internal sealed class ConstVarVisitor : IVisitor<ConstVarNode> 
{
    public static ConstVarVisitor Instance => _instance;
    private static ConstVarVisitor _instance = new ConstVarVisitor();
    private ConstVarVisitor() {}
    public async Task<ConstVarNode?> VisitAsync(Parser parser) 
    {
        if (parser.tokens.Current.Kind != SyntaxKind.ConstToken) 
        {
            await parser.diagnostics.ReportUnsuitableSyntaxToken(
                parser.tokens.Current.Metadata.Line, parser.tokens.Current, SyntaxKind.ConstToken);

            return null;
        }
        parser.tokens.Increment();

        TypeNode? type = await VisitorFactory.GetVisitorFor<TypeNode>(parser.context)!.VisitAsync(parser);

        string? name = SyntaxHelper.ParseVariableName(parser);        
        if (name != null) parser.tokens.Increment();

        // null as const value for now, later on parsing expressions make literal checks
        ConstVarNode curr = new ConstVarNode(name!, type!);

        IExpressionVisitor<AssignExpression> visitor = 
            (IExpressionVisitor<AssignExpression>)VisitorFactory.GetVisitorFor<AssignExpression>(parser.context)!;

        AssignExpression? exp = await visitor.VisitAsync(parser, new RefValueNode(curr));

        if (exp == null || !exp.Right.Type.ImplicitTypeCheck(type!)) 
        {
            await parser.diagnostics.ReportTypeCheck(parser.tokens.Current.Metadata.Line);
        }
        if (parser.tokens.Current.Kind != SyntaxKind.SemicolonToken) 
        {
            await parser.diagnostics.ReportSemicolonExpected(parser.tokens.Current.Metadata.Line);
        } else parser.tokens.Increment();

        return curr;
    }
}