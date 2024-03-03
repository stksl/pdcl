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

        TypeNode? type = await VisitorFactory.GetVisitorFor<TypeNode>()!.VisitAsync(parser);

        string? name = SyntaxHelper.ParseVariableName(parser);        
        if (name != null) parser.tokens.Increment();

        ConstVarNode curr = new ConstVarNode(name!, type!, null!, parser.tokens.Index);

        IExpressionVisitor<AssignExpression> visitor = 
            (IExpressionVisitor<AssignExpression>)VisitorFactory.GetVisitorFor<AssignExpression>()!;

        AssignExpression? exp = await visitor.VisitAsync(parser, new RefValueNode(curr, parser.tokens.Index));

        if (parser.tokens.Current.Kind != SyntaxKind.SemicolonToken) 
        {
            await parser.diagnostics.ReportSemicolonExpected(parser.tokens.Current.Metadata.Line);
        }

        curr.ConstValue = exp != null ? SyntaxHelper.ParseLiteralExpression(exp)! : null!;
        return curr;
    }
}