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
        var root = (parser.tree!.Root as SyntaxTree.ApplicationContextNode)!;

        // checks for all possible top-level nodes, visiting them 
        
        while (parser.tokens.Current.Kind == SyntaxKind.UseToken) 
        {
            UseNode? use = await VisitorFactory.GetVisitorFor<UseNode>(parser.context)!.VisitAsync(parser);
            if (use != null) root.addChild(use);
        }

        while (parser.tokens.Index < parser.tokens.Length) 
        {
            SyntaxNode? node = null;
            switch(parser.tokens.Current.Kind) 
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
                    await parser.diagnostics.ReportUnsuitableSyntaxToken(parser.tokens.Current.Metadata.Line, 
                        parser.tokens.Current, SyntaxKind.TextToken);
                    break;
            }

            if (node != null) root.addChild(node);
        }
        return root;
    }
}