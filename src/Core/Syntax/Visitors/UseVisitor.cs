using System.Text;

namespace Pdcl.Core.Syntax;

internal sealed class UseVisitor : IVisitor<UseNode>
{
    public static UseVisitor Instance => _instance;
    private static UseVisitor _instance = new UseVisitor();
    private UseVisitor() { }
    public Task<UseNode?> VisitAsync(Parser parser)
    {
        if (parser.IsConsumeMissed(SyntaxKind.UseToken))
            return Task.FromResult<UseNode?>(null);
        
        string nsName = SyntaxHelper.ParseNamespaceName(parser);

        parser.ConsumeToken(SyntaxKind.SemicolonToken);
        return Task.FromResult<UseNode?>(new UseNode(nsName));
    }
}