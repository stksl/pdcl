namespace Pdcl.Core.Syntax;

internal static class SyntaxHelper 
{
    public static string? ParseVariableName(Parser parser) 
    {
        if (parser.tokens.Current.Kind != SyntaxKind.TextToken) 
        {
            parser.diagnostics.ReportUnsuitableSyntaxToken(
                parser.tokens.Current.Metadata.Line, parser.tokens.Current, SyntaxKind.TextToken);
            return null;
        }
        if (parser.GetCurrentTableNode().Table!.GetSymbol(parser.tokens.Current.Metadata.Raw, SymbolType.LocalVar).HasValue) 
        {
            parser.diagnostics.ReportAlreadyDefined(
                parser.tokens.Current.Metadata.Line, parser.tokens.Current.Metadata.Raw);
            return null;
        }

        return parser.tokens.Current.Metadata.Raw;
    }
    public static LiteralValue? ParseLiteralExpression(ExpressionNode? exp) 
    {
        throw new NotImplementedException();
    }
}