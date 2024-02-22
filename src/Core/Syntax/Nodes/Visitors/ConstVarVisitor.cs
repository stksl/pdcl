namespace Pdcl.Core.Syntax;

internal sealed class ConstVarVisitor : IVisitor<ConstVarNode> 
{
    public async Task<ConstVarNode?> VisitAsync(Parser parser) 
    {
        if (parser.CurrentToken.Kind != SyntaxKind.ConstToken) 
        {
            parser.diagnostics.ReportUnsuitableSyntaxToken(
                parser.CurrentToken.Metadata.Line, parser.CurrentToken, SyntaxKind.ConstToken);
            return null;
        }
        parser.tokensInd++;

        VariableType? type = await VisitorFactory.GetVisitorFor<VariableType>()!.VisitAsync(parser);
        if (type == null) return null; // supposably VariableTypeVisitor had already reported an error.


        throw new NotImplementedException();
    }
}