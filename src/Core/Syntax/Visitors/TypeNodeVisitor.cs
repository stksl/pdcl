namespace Pdcl.Core.Syntax;

internal sealed class TypeNodeVisitor : IVisitor<TypeNode> 
{
    public static TypeNodeVisitor Instance => _instance;
    private static TypeNodeVisitor _instance = new TypeNodeVisitor();
    private TypeNodeVisitor() {}
    public async Task<TypeNode?> VisitAsync(Parser parser) 
    {
        if (parser.CurrentToken.Kind != SyntaxKind.TextToken) 
        {
            await parser.diagnostics.ReportUnsuitableSyntaxToken(parser.CurrentToken.Metadata.Line, 
                parser.CurrentToken, SyntaxKind.TextToken);
            return null;
        }
        
        string typeName = parser.CurrentToken.Metadata.Raw;
        if (SyntaxFacts.IsPrimitiveType(typeName, out PrimitiveTypeNode.PrimitiveTypes? type)) 
        {
            parser.ConsumeToken();
            return new PrimitiveTypeNode(type!.Value);
        }
        
        Symbol? symbol = parser.GetCurrentTableNode().GetSymbol(typeName, SymbolType.TypeDefinition);
        if (!symbol.HasValue) 
        {
            await parser.diagnostics.ReportUnknownSymbol(parser.CurrentToken.Metadata.Line, typeName);
            return null;
        }

        parser.ConsumeToken();
        return new TypeNode((TypeDeclarationNode)symbol.Value.Node);
    }
}