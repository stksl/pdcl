using System.Text;

namespace Pdcl.Core.Syntax;

internal sealed class UseVisitor : IVisitor<UseNode>
{
    public static UseVisitor Instance => _instance;
    private static UseVisitor _instance = new UseVisitor();
    private UseVisitor() { }
    public Task<UseNode?> VisitAsync(Parser parser)
    {
        if (parser.CurrentToken.Kind != SyntaxKind.UseToken)
        {
            parser.diagnostics.ReportUnsuitableSyntaxToken(
                parser.CurrentToken.Metadata.Line, parser.CurrentToken, SyntaxKind.UseToken);
            return Task.FromResult<UseNode?>(null);
        }
        parser.tokensInd++;
        StringBuilder nsIdentifier = new StringBuilder();
        SyntaxToken prevToken = parser.CurrentToken;
        while (parser.CurrentToken.Kind == SyntaxKind.TextToken || parser.CurrentToken.Kind == SyntaxKind.DotToken) 
        {
            // two dot tokens together
            if (prevToken.Kind == parser.CurrentToken.Kind && prevToken.Kind == SyntaxKind.DotToken) 
            {
                parser.diagnostics.ReportIncorrectNamespaceSyntax(parser.CurrentToken.Metadata.Line, nsIdentifier.ToString());
                return Task.FromResult<UseNode?>(null);
            }
            nsIdentifier.Append(parser.CurrentToken.Metadata.Raw);
        }
        if (parser.CurrentToken.Kind != SyntaxKind.SemicolonToken) 
        {
            parser.diagnostics.ReportSemicolonExpected(parser.CurrentToken.Metadata.Line);
            return Task.FromResult<UseNode?>(null);
        }
        parser.tokensInd++;
        return Task.FromResult<UseNode?>(new UseNode(nsIdentifier.ToString(), parser.tokensInd));
    }
}