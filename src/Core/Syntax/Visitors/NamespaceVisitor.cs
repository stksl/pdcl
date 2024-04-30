namespace Pdcl.Core.Syntax;

internal sealed class NamespaceVisitor : IVisitor<NamespaceNode> 
{
    public static NamespaceVisitor Instance => _instance;
    private static NamespaceVisitor _instance = new NamespaceVisitor();
    private NamespaceVisitor() {}

    public async Task<NamespaceNode?> VisitAsync(Parser parser) 
    {
        if (parser.IsConsumeMissed(SyntaxKind.NamespaceToken)) 
        {
            return null;
        }

        // all namespaces will start with '@' prefix as a unique identifier in the table tree
        string name = "@" + SyntaxHelper.ParseNamespaceName(parser);
        SymbolTreeNode nsTreeNode = parser.AssemblyInfo.TableTree.Root.Children.TryGetValue(name, out SymbolTreeNode? nsTNode) 
        ? nsTNode 
        : parser.AssemblyInfo.TableTree.Root.Children[name] = new SymbolTreeNode(parser.AssemblyInfo.TableTree.Root, new SymbolTable(), name);

        NamespaceNode nsNode = new NamespaceNode(name);

        parser.ConsumeToken(SyntaxKind.OpenBraceToken);
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
                        nsTreeNode.Table!.StoreSymbol(
                            new Symbol(constVar.Name, SymbolType.Variable, 0, constVar));
                    }
                    node = constVar;
                    break;
                case SyntaxKind.TextToken:
                        // supposedly a function declaration
                    break;
                case SyntaxKind.CloseBraceToken:
                    parser.ConsumeToken(throwOnEOF: false);
                    goto _escape;
                case SyntaxKind.StructToken:
                    break;
                default:
                    await parser.diagnostics.ReportUnsuitableSyntaxToken(parser.CurrentToken.Metadata.Line, 
                        parser.CurrentToken, SyntaxKind.TextToken);
                    break;
            }

            if (node != null) nsNode.addChild(node);
        }

    _escape:

        return nsNode;
    }
}