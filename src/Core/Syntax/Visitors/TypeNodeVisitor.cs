namespace Pdcl.Core.Syntax;

internal sealed class TypeNodeVisitor : IVisitor<TypeNode> 
{
    public static TypeNodeVisitor Instance => _instance;
    private static TypeNodeVisitor _instance = new TypeNodeVisitor();
    private TypeNodeVisitor() {}
    public async Task<TypeNode?> VisitAsync(Parser parser) 
    {
        if (!SyntaxHelper.CheckTokens(parser, SyntaxKind.TextToken))
            return null;

        string typeName = parser.tokens.Current.Metadata.Raw;
        if (SyntaxFacts.IsPrimitiveType(typeName, out PrimitiveTypeNode.PrimitiveTypes? type)) 
        {
            parser.tokens.Increment();

            return new PrimitiveTypeNode(type!.Value);
        }
        
        Symbol? symbol = parser.GetCurrentTableNode().GetSymbol(typeName, SymbolType.TypeDefinition);
        if (!symbol.HasValue) 
        {
            await parser.diagnostics.ReportUnknownSymbol(parser.tokens.Current.Metadata.Line, typeName);
            return null;
        }
        parser.tokens.Increment();
        return new TypeNode((TypeDeclarationNode)symbol.Value.Node);
    }
}