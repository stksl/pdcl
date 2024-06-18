using System.Text;
namespace Pdcl.Core.Syntax;

internal static class ParserExtensions
{
    public static string ParseNamespaceName(this Parser parser)
    {
        StringBuilder sb = new StringBuilder();
        while (parser.CurrentToken.Kind == SyntaxKind.TextToken)
        {
            sb.Append(parser.CurrentToken.Metadata.Raw);
            if (parser.ConsumeToken().Kind == SyntaxKind.DotToken)
            {
                sb.Append('.');
                parser.ConsumeToken();
            }
        }
        return sb.ToString();
    }
    public static string? ParseVariableName(this Parser parser)
    {
        if (parser.CurrentToken.Kind != SyntaxKind.TextToken)
        {
            parser.diagnostics.ReportUnsuitableSyntaxToken(
                parser.CurrentToken.Metadata.Line, parser.CurrentToken.Kind, SyntaxKind.TextToken);
            return null;
        }
        if (parser.GetCurrentTableNode().Table!.GetSymbol(parser.CurrentToken.Metadata.Raw, SymbolType.Variable).HasValue)
        {
            parser.diagnostics.ReportAlreadyDefined(
                parser.CurrentToken.Metadata.Line, parser.CurrentToken.Metadata.Raw);
            return null;
        }

        return parser.CurrentToken.Metadata.Raw;
    }
}