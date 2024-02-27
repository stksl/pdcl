namespace Pdcl.Core.Syntax;

internal sealed class ValueNodeVisitor : IVisitor<ValueNode> 
{
    public async Task<ValueNode?> VisitAsync(Parser parser) 
    {
        if (SyntaxFacts.IsLiteralKind(parser.CurrentToken.Kind)) 
            return await VisitLiteralAsync(parser);
        // BUILD from this lvalue
        if (parser.CurrentToken.Kind == SyntaxKind.TextToken)
            return await VisitRefAsync(parser);

        // not a value node?

    }
    private Task<LiteralValue> VisitLiteralAsync(Parser parser) 
    {
        LiteralValue.LiteralType literalType = parser.CurrentToken.Kind switch
        {
            SyntaxKind.StringLiteral => LiteralValue.LiteralType.String, 
            SyntaxKind.CharLiteral => LiteralValue.LiteralType.Character,
            _ => LiteralValue.LiteralType.Number 
        };
        parser.tokensInd++;
        return Task.FromResult(new LiteralValue(parser.CurrentToken, literalType, parser.tokensInd));
    }
    private async Task<ValueNode?> VisitRefAsync(Parser parser) 
    {
        string name = parser.CurrentToken.Metadata.Raw;

        Symbol? symbol = parser.GetCurrentTable().GetSymbol(name, SymbolType.LocalVar) ?? 
            parser.GetCurrentTable().GetSymbol(name, SymbolType.Field);
        parser.tokensInd++;

        if (!symbol.HasValue) 
        {
            if (parser.CurrentToken.Kind == SyntaxKind.OpenParentheseToken)
                return await VisitInvokeAsync(parser);
            parser.diagnostics.ReportUnknownSymbol(parser.CurrentToken.Metadata.Line, name);
            return null;
        }
        return new RefValueNode((ValueHolderNode)symbol.Value.Node, parser.tokensInd);
    }
    private async Task<FuncInvokeValue?> VisitInvokeAsync(Parser parser) 
    {
        parser.tokensInd++;
        while (parser.CurrentToken.Kind != SyntaxKind.CloseParentheseToken) 
        {
            ValueNode? passedArg = await VisitorFactory.GetVisitorFor<ValueNode>()!.VisitAsync(parser);


        }
    }
}