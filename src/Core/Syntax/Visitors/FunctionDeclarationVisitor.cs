using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace Pdcl.Core.Syntax;

internal sealed class FunctionDeclarationVisitor : IVisitor<FunctionDeclaration> 
{
    private static FunctionDeclarationVisitor _instance = new FunctionDeclarationVisitor();
    public static FunctionDeclarationVisitor Instance => _instance;
    private FunctionDeclarationVisitor() {}

    public Task<FunctionDeclaration?> VisitAsync(Parser parser) 
    {
        throw new NotImplementedException("Use overloaded method instead.");
    }
    public async Task<FunctionDeclaration?> VisitAsync(Parser parser, TypeNode retType, string name) 
    {
        if (parser.IsConsumeMissed(SyntaxKind.OpenParentheseToken)) 
        {
            return null;
        }
        parser.currPath += "/" + name;
        Dictionary<string, TypeNode> args = new Dictionary<string, TypeNode>();
        while (parser.CurrentToken.Kind != SyntaxKind.CloseParentheseToken) 
        {
            TypeNode? argType = await VisitorFactory.GetVisitorFor<TypeNode>(parser.context)!.VisitAsync(parser);
            string? argName = parser.CurrentToken.Metadata.Raw;
            parser.ConsumeToken(SyntaxKind.TextToken);

            if (args.ContainsKey(argName))
                await parser.diagnostics.ReportAlreadyDefined(parser.CurrentToken.Metadata.Line, argName);
            
            if (parser.CurrentToken.Kind != SyntaxKind.CloseParentheseToken)
                parser.ConsumeToken(SyntaxKind.CommaToken);
            
            args[argName] = argType!;
            parser.GetCurrentTableNode().Table!.StoreSymbol(new Symbol(argName, SymbolType.Argument, 0, argType!));
        }

        parser.ConsumeToken();

        FunctionSignature sig = new FunctionSignature(name, retType, args.ToImmutableDictionary());

        parser.ConsumeToken(SyntaxKind.OpenBraceToken);

        FunctionBody? body = await visitBodyAsync(parser);

        parser.currPath = parser.currPath.Remove(parser.currPath.Length - name.Length);
        parser.ConsumeToken(SyntaxKind.CloseBraceToken);

        return new FunctionDeclaration(sig, body!, parser.currPath + "/" + name);
    }

    private async Task<FunctionBody?> visitBodyAsync(Parser parser) 
    {
        
    }
}