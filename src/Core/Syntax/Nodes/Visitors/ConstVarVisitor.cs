namespace Pdcl.Core.Syntax;

internal sealed class ConstVarVisitor : IVisitor<ConstVarNode> 
{
    public static ConstVarVisitor Instance => _instance;
    private static ConstVarVisitor _instance = new ConstVarVisitor();
    private ConstVarVisitor() {}
    public async Task<ConstVarNode?> VisitAsync(Parser parser) 
    {
        if (parser.CurrentToken.Kind != SyntaxKind.ConstToken) 
        {
            parser.diagnostics.ReportUnsuitableSyntaxToken(
                parser.CurrentToken.Metadata.Line, parser.CurrentToken, SyntaxKind.ConstToken);
            return null;
        }
        parser.tokensInd++;

        TypeNode? type = await VisitorFactory.GetVisitorFor<TypeNode>()!.VisitAsync(parser);

        string? name = SyntaxHelper.ParseVariableName(parser);        

        AssignExpression? assign = 
            await VisitorFactory.GetVisitorFor<AssignExpression>()!.VisitAsync(parser); // will throw!

        LiteralValue? val = SyntaxHelper.ParseLiteralExpression(assign); // will throw !
        if (parser.CurrentToken.Kind != SyntaxKind.SemicolonToken) 
        {
            parser.diagnostics.ReportSemicolonExpected(parser.CurrentToken.Metadata.Line);
        }

        return new ConstVarNode(name!, type!, val!, parser.tokensInd);
    }
}