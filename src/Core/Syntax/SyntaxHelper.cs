using System.Text;
using Pdcl.Core.Syntax.Semantics;

namespace Pdcl.Core.Syntax;

internal static class SyntaxHelper
{
    /// <summary>
    /// Checks current token in the stream and nexts for <paramref name="kinds"/> equality, reports an error if needed
    /// </summary>
    /// <param name="parser"></param>
    /// <param name="kinds"></param>
    /// <returns></returns>
    public static bool CheckTokens(Parser parser, params SyntaxKind[] kinds)
    {
        for (int i = 0; i < kinds.Length; i++, parser.tokens.Increment())
        {
            if (parser.tokens.Current.Kind != kinds[i])
            {
                parser.diagnostics.ReportUnsuitableSyntaxToken(parser.tokens.Current.Metadata.Line,
                    parser.tokens.Current, kinds[i]);
                return false;
            }
        }
        parser.tokens.SetOffset(-kinds.Length);
        return true;
    }
    public static string ParseNamespaceName(Parser parser)
    {
        StringBuilder sb = new StringBuilder();
        while (CheckTokens(parser, SyntaxKind.TextToken))
        {
            sb.Append(parser.tokens.Current.Metadata.Raw);
            parser.tokens.Increment();
            if (parser.tokens.Current.Kind == SyntaxKind.DotToken)
            {
                sb.Append('.');
                parser.tokens.Increment();
            }
        }
        if (parser.tokens.Current.Kind == SyntaxKind.DotToken)
        {
            parser.diagnostics.ReportIncorrectNamespaceSyntax(parser.tokens.Current.Metadata.Line,
                sb.ToString() + "."
            );
        }
        return sb.ToString();
    }
    public static string? ParseVariableName(Parser parser)
    {
        if (parser.tokens.Current.Kind != SyntaxKind.TextToken)
        {
            parser.diagnostics.ReportUnsuitableSyntaxToken(
                parser.tokens.Current.Metadata.Line, parser.tokens.Current, SyntaxKind.TextToken);
            return null;
        }
        if (parser.GetCurrentTableNode().Table!.GetSymbol(parser.tokens.Current.Metadata.Raw, SymbolType.Variable).HasValue)
        {
            parser.diagnostics.ReportAlreadyDefined(
                parser.tokens.Current.Metadata.Line, parser.tokens.Current.Metadata.Raw);
            return null;
        }

        return parser.tokens.Current.Metadata.Raw;
    }
}