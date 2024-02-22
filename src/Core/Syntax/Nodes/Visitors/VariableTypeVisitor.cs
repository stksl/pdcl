namespace Pdcl.Core.Syntax;

internal sealed class VariableTypeVisitor : IVisitor<VariableType> 
{
    public Task<VariableType?> VisitAsync(Parser parser) 
    {
        if (parser.CurrentToken.Kind != SyntaxKind.TextToken) 
        {
            parser.diagnostics.ReportUnsuitableSyntaxToken(
                parser.CurrentToken.Metadata.Line, parser.CurrentToken, SyntaxKind.TextToken);
            return Task.FromResult<VariableType?>(null);
        }

        string typeName = parser.CurrentToken.Metadata.Raw;

        Parser.Symbol? symbol = parser.GlobalTable.GetSymbol(typeName, Parser.SymbolType.TypeDefinition);
        if (!symbol.HasValue) 
        {
            parser.diagnostics.ReportUnknownType(parser.CurrentToken.Metadata.Line, typeName);
            return Task.FromResult<VariableType?>(null);
        }

        return Task.FromResult<VariableType?>(
            new VariableType(typeName, (TypeDeclarationNode)symbol.Value.Node, parser.tokensInd));
    }
}