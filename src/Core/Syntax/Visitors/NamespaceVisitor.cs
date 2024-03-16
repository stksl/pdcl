namespace Pdcl.Core.Syntax;

internal sealed class NamespaceVisitor : IVisitor<NamespaceNode> 
{
    public static NamespaceVisitor Instance => _instance;
    private static NamespaceVisitor _instance = new NamespaceVisitor();
    private NamespaceVisitor() {}

    public async Task<NamespaceNode?> VisitAsync(Parser parser) 
    {
        if (!SyntaxHelper.CheckTokens(parser, SyntaxKind.NamespaceToken, SyntaxKind.TextToken))
            return null;

        parser.tokens.Increment();

        // all namespaces will start with '@' prefix as a unique identifier in the table tree
        string name = "@" + SyntaxHelper.ParseNamespaceName(parser);
        SymbolTreeNode nsTreeNode = parser.AssemblyInfo.TableTree.Root.Children.TryGetValue(name, out SymbolTreeNode? nsTNode) 
        ? nsTNode 
        : parser.AssemblyInfo.TableTree.Root.Children[name] = new SymbolTreeNode(parser.AssemblyInfo.TableTree.Root, new SymbolTable(), name);

        NamespaceNode nsNode = new NamespaceNode(name);

        if (!SyntaxHelper.CheckTokens(parser, SyntaxKind.OpenBraceToken)) 
        {
            await parser.diagnostics.ReportUnsuitableSyntaxToken(parser.tokens.Current.Metadata.Line,
                parser.tokens.Current, SyntaxKind.OpenBraceToken);
        }
        else parser.tokens.Increment();

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
                        nsTreeNode.Table!.StoreSymbol(
                            new Symbol(constVar.Name, SymbolType.Variable, 0, constVar));
                    }
                    node = constVar;
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

            if (node != null) nsNode.addChild(node);
        }

        if (!SyntaxHelper.CheckTokens(parser, SyntaxKind.CloseBraceToken)) 
        {
            await parser.diagnostics.ReportUnsuitableSyntaxToken(parser.tokens.Current.Metadata.Line,
                parser.tokens.Current, SyntaxKind.CloseBraceToken);
        } else parser.tokens.Increment();
        return nsNode;
    }
}