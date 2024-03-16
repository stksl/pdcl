using System.Text;

namespace Pdcl.Core.Syntax;

internal sealed class UseVisitor : IVisitor<UseNode>
{
    public static UseVisitor Instance => _instance;
    private static UseVisitor _instance = new UseVisitor();
    private UseVisitor() { }
    public async Task<UseNode?> VisitAsync(Parser parser)
    {
        if (!SyntaxHelper.CheckTokens(parser, SyntaxKind.UseToken))
            return null;
        parser.tokens.Increment();
        StringBuilder nsIdentifier = new StringBuilder();
        SyntaxToken prevToken = parser.tokens.Current;
        while (parser.tokens.Current.Kind == SyntaxKind.TextToken || parser.tokens.Current.Kind == SyntaxKind.DotToken) 
        {
            // two dot tokens together
            if (prevToken.Kind == parser.tokens.Current.Kind && prevToken.Kind == SyntaxKind.DotToken) 
            {
                await parser.diagnostics.ReportIncorrectNamespaceSyntax(parser.tokens.Current.Metadata.Line, nsIdentifier.ToString());
                return null;
            }
            nsIdentifier.Append(parser.tokens.Current.Metadata.Raw);
        }
        if (parser.tokens.Current.Kind != SyntaxKind.SemicolonToken) 
        {
            await parser.diagnostics.ReportSemicolonExpected(parser.tokens.Current.Metadata.Line);
        }
        parser.tokens.Increment();
        return new UseNode(nsIdentifier.ToString());
    }
}