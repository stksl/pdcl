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
        SyntaxTree.ApplicationContextNode root = parser.tree!.Root;

        // checks for all possible top-level nodes, visiting them 
        
        parser.ConsumeToken(); // consuming initial token
        while (parser.CurrentToken.Kind == SyntaxKind.UseToken) 
        {
            UseNode? use = await VisitorFactory.GetVisitorFor<UseNode>(parser.context)!.VisitAsync(parser);
            if (use != null) root.addChild(use);
        }

        while (!parser.IsEOF) 
        {
            SyntaxNode? node = null;
            switch(parser.CurrentToken.Kind) 
            {
                // global constant variable declaration
                case SyntaxKind.ConstToken:
                    ConstVarNode? constVar = await VisitorFactory.GetVisitorFor<ConstVarNode>(parser.context)!.VisitAsync(parser);
                    if (constVar != null) 
                    {
                        parser.AssemblyInfo.TableTree.Root.Table!.StoreSymbol(
                            new Symbol(constVar.Name, SymbolType.Variable, 0, constVar));
                    }
                    node = constVar;
                    break;
                case SyntaxKind.NamespaceToken:
                    node = await VisitorFactory.GetVisitorFor<NamespaceNode>(parser.context)!.VisitAsync(parser);
                    break;
                // supposably a function declaration
                case SyntaxKind.TextToken:
                    break;
                case SyntaxKind.StructToken:
                    break;
                default:
                    await parser.diagnostics.ReportUnsuitableSyntaxToken(parser.CurrentToken.Metadata.Line, 
                        parser.CurrentToken, SyntaxKind.TextToken);
                    break;
            }

            if (node != null) root.addChild(node);
        }
        return root;
    }
}