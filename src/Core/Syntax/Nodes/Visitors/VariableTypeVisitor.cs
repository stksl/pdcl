namespace Pdcl.Core.Syntax;

internal sealed class VariableTypeVisitor : IVisitor<TypeNode> 
{
    public static VariableTypeVisitor Instance => _instance;
    private static VariableTypeVisitor _instance = new VariableTypeVisitor();
    public Task<TypeNode?> VisitAsync(Parser parser) 
    {
        if (parser.CurrentToken.Kind != SyntaxKind.TextToken) 
        {
            parser.diagnostics.ReportUnsuitableSyntaxToken(
                parser.CurrentToken.Metadata.Line, parser.CurrentToken, SyntaxKind.TextToken);
            return Task.FromResult<TypeNode?>(null);
        }

        string typeName = parser.CurrentToken.Metadata.Raw;

        Symbol? symbol = parser.GlobalTable.GetSymbol(typeName, SymbolType.TypeDefinition);
        if (!symbol.HasValue) 
        {
            parser.diagnostics.ReportUnknownSymbol(parser.CurrentToken.Metadata.Line, typeName);
            return Task.FromResult<TypeNode?>(null);
        }
        parser.tokensInd++;
        return Task.FromResult<TypeNode?>(
            new TypeNode(typeName, (TypeDeclarationNode)symbol.Value.Node, parser.tokensInd));
    }
}