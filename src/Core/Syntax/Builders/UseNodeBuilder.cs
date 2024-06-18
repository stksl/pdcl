namespace Pdcl.Core.Syntax;

internal sealed partial class SyntaxNodeBuilder : ISyntaxNodeBuilder
{
    public UseNode UseNode(SyntaxKind useToken, string nsName, SyntaxKind semicolonToken) 
    {
        if (useToken != SyntaxKind.UseToken)
            _diagnosticHandler.ReportUnsuitableSyntaxToken(_stream.line, useToken, SyntaxKind.UseToken);
        if (semicolonToken != SyntaxKind.SemicolonToken)
            _diagnosticHandler.ReportUnsuitableSyntaxToken(_stream.line, semicolonToken, SyntaxKind.SemicolonToken);
        
        return new UseNode(nsName);
    }
}