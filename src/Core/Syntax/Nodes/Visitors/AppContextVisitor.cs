using System.Numerics;

namespace Pdcl.Core.Syntax;

internal sealed class ApplicationContextVisitor : IVisitor<SyntaxTree.ApplicationContextNode> 
{
    private static ApplicationContextVisitor _instance = new ApplicationContextVisitor();
    public static ApplicationContextVisitor Instance => _instance;
    private ApplicationContextVisitor() {}
    public async Task<SyntaxTree.ApplicationContextNode?> VisitAsync(Parser parser) 
    {
        // getting the root as an app context, directly changing it
        var root = (parser._tree!.Root as SyntaxTree.ApplicationContextNode)!;

        // checks for all possible top-level nodes, visiting them 
        
        while (parser.CurrentToken.Kind == SyntaxKind.UseToken) 
        {
            UseNode? use = await VisitorFactory.GetVisitorFor<UseNode>()!.VisitAsync(parser);
            if (use != null) root.addChild(use);
        }

        switch(parser.CurrentToken.Kind) 
        {
            // global constant variable declaration
            case SyntaxKind.ConstToken:
                
                break;
            // supposably a function declaration
            case SyntaxKind.TextToken:
                break;
            case SyntaxKind.StructToken:
                break;
        }

        return root;
    }
}