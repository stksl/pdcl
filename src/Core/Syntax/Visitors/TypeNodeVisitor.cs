namespace Pdcl.Core.Syntax;

internal sealed class TypeNodeVisitor : IVisitor<TypeNode> 
{
    public static TypeNodeVisitor Instance => _instance;
    private static TypeNodeVisitor _instance = new TypeNodeVisitor();
    public async Task<TypeNode?> VisitAsync(Parser parser) 
    {
        if (parser.tokens.Current.Kind != SyntaxKind.TextToken) 
        {
            await parser.diagnostics.ReportUnsuitableSyntaxToken(
                parser.tokens.Current.Metadata.Line, parser.tokens.Current, SyntaxKind.TextToken);
            return null;
        }

        string typeName = parser.tokens.Current.Metadata.Raw;
        if (SyntaxFacts.IsPrimitiveType(typeName, out PrimitiveTypeNode.PrimitiveTypes? type)) 
        {
            parser.tokens.Increment();
            return new PrimitiveTypeNode(type!.Value, typeName, null!, parser.tokens.Index);
        }
        
        Symbol? symbol = parser.GetCurrentTableNode().GetSymbol(typeName, SymbolType.TypeDefinition);
        if (!symbol.HasValue) 
        {
            await parser.diagnostics.ReportUnknownSymbol(parser.tokens.Current.Metadata.Line, typeName);
            return null;
        }
        parser.tokens.Increment();
        return new TypeNode(typeName, (TypeDeclarationNode)symbol.Value.Node, parser.tokens.Index);
    }
}