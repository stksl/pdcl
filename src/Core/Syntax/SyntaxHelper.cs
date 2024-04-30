using System.Text;
using Pdcl.Core.Syntax.Semantics;

namespace Pdcl.Core.Syntax;

internal static class SyntaxHelper
{
    public static string ParseNamespaceName(Parser parser)
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
        if (parser.CurrentToken.Kind == SyntaxKind.DotToken)
        {
            parser.diagnostics.ReportIncorrectNamespaceSyntax(parser.CurrentToken.Metadata.Line,
                sb.ToString() + "."
            );
        }
        return sb.ToString();
    }
    public static string? ParseVariableName(Parser parser)
    {
        if (parser.CurrentToken.Kind != SyntaxKind.TextToken)
        {
            parser.diagnostics.ReportUnsuitableSyntaxToken(
                parser.CurrentToken.Metadata.Line, parser.CurrentToken, SyntaxKind.TextToken);
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