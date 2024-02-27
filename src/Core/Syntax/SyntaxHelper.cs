namespace Pdcl.Core.Syntax;

internal static class SyntaxHelper 
{
    public static string? ParseVariableName(Parser parser) 
    {
        if (parser.CurrentToken.Kind != SyntaxKind.TextToken) 
        {
            parser.diagnostics.ReportUnsuitableSyntaxToken(
                parser.CurrentToken.Metadata.Line, parser.CurrentToken, SyntaxKind.TextToken);
            return null;
        }
        if (parser.GetCurrentTable().GetSymbol(parser.CurrentToken.Metadata.Raw, SymbolType.LocalVar).HasValue) 
        {
            parser.diagnostics.ReportAlreadyDefiend(
                parser.CurrentToken.Metadata.Line, parser.CurrentToken.Metadata.Raw);
            return null;
        }

        return parser.tokens[parser.tokensInd++].Metadata.Raw;
    }
    public static LiteralValue? ParseLiteralExpression(ExpressionNode? exp) 
    {
        throw new NotImplementedException();
    }
}