using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace Pdcl.Core.Syntax;

internal sealed partial class ValueNodeVisitor : IVisitor<ValueNode>
{
    public static ValueNodeVisitor Instance => _instance;
    private static ValueNodeVisitor _instance = new();
    private ValueNodeVisitor() {} 
    public async Task<ValueNode?> VisitAsync(Parser parser)
    {
        ValueNode? result = null;
        switch(parser.tokens.Current.Kind) 
        {
            case > 0 when SyntaxFacts.IsLiteralKind(parser.tokens.Current.Kind):
                result = await VisitLiteralAsync(parser);
            break;
            case SyntaxKind.OpenParentheseToken:
                result = await VisitorFactory.GetVisitorFor<ParenthesizedExpression>()!.VisitAsync(parser);
                break;
            case SyntaxKind.TextToken:
                result = parser.tokens.Check(1)!.Value.Kind switch 
                {
                    SyntaxKind.OpenParentheseToken => await VisitFuncInvokeAsync(parser, parser.GetCurrentTableNode()),
                    SyntaxKind.DotToken => await VisitMemberExpAsync(parser),
                    _ => await VisitRefAsync(parser) 
                };
                break;
            default:
                await parser.diagnostics.ReportUnkownOperandSyntax(parser.tokens.Current.Metadata.Line,
                parser.tokens.Current.Metadata.Raw);
                break;
        }
        

        if (result != null && SyntaxFacts.IsBinaryOperator(parser.tokens.Current.Kind, out _)) 
        {
            IExpressionVisitor<BinaryExpression> visitor = 
                (IExpressionVisitor<BinaryExpression>)VisitorFactory.GetVisitorFor<BinaryExpression>()!;
            result = await visitor.VisitAsync(parser, result);
        }
        return result;
    }
    private Task<LiteralValue> VisitLiteralAsync(Parser parser)
    {
        LiteralValue.LiteralType literalType = parser.tokens.Current.Kind switch
        {
            SyntaxKind.StringLiteral => LiteralValue.LiteralType.String,
            SyntaxKind.CharLiteral => LiteralValue.LiteralType.Character,
            _ => LiteralValue.LiteralType.Number
        };
        
        var res = Task.FromResult(new LiteralValue(parser.tokens.Current, literalType, parser.tokens.Index));
        parser.tokens.Increment();
        return res;
    }
    private async Task<RefValueNode?> VisitRefAsync(Parser parser)
    {
        string name = parser.tokens.Current.Metadata.Raw;

        Symbol? symbol = parser.GetCurrentTableNode().Table!.GetSymbol(name, SymbolType.LocalVar) ??
            parser.GetCurrentTableNode().Table!.GetSymbol(name, SymbolType.Field);
        if (!symbol.HasValue)
        {
            await parser.diagnostics.ReportUnknownSymbol(parser.tokens.Current.Metadata.Line, name);
            return null;
        }
        parser.tokens.Increment();

        return new RefValueNode((ValueHolderNode)symbol.Value.Node, parser.tokens.Index);
    }
    private async Task<FunctionInvoke?> VisitFuncInvokeAsync(Parser parser, SymbolTreeNode treeNode)
    {
        Symbol? function = treeNode.GetSymbol(parser.tokens.Current.Metadata.Raw, SymbolType.Function);
        if (!function.HasValue)
        {
            await parser.diagnostics.ReportUnknownSymbol(parser.tokens.Current.Metadata.Line,
                parser.tokens.Current.Metadata.Raw);
            return null;
        }
        parser.tokens.SetOffset(2); // skipping name and '('
        List<ValueNode> passedVals = new List<ValueNode>();
        int index = 0;
        while (parser.tokens.Current.Kind != SyntaxKind.CloseParentheseToken)
        {
            ValueNode? passedVal = await VisitorFactory.GetVisitorFor<ValueNode>()!.VisitAsync(parser);
            if (passedVal != null) passedVals.Add(passedVal);

            if (parser.tokens.Current.Kind != SyntaxKind.CommaToken && parser.tokens.Current.Kind != SyntaxKind.CloseParentheseToken)
            {
                await parser.diagnostics.ReportNoArgSeparator(parser.tokens.Current.Metadata.Line);
            }

            index++;
        }
        parser.tokens.Increment();
        return new FunctionInvoke((FunctionDeclaration)function.Value.Node, passedVals.ToImmutableArray(),
            parser.tokens.Index);
    }
    private async Task<MemberExpression?> VisitMemberExpAsync(Parser parser) 
    {
        RefValueNode? refNode = await VisitRefAsync(parser);

        parser.tokens.SetOffset(2);

        MemberExpressionNode? root = null;
        if (refNode != null) root = await visitMemberExp(parser, refNode.Holder);
        
        return new MemberExpression(refNode!, root!, parser.tokens.Index);
    }
    private async Task<MemberExpressionNode?> visitMemberExp(Parser parser, ValueHolderNode holder) 
    {
        if (parser.tokens.Current.Kind != SyntaxKind.TextToken) 
        {
            await parser.diagnostics.ReportUnsuitableSyntaxToken(parser.tokens.Current.Metadata.Line, 
                parser.tokens.Current, SyntaxKind.TextToken);
            return null;
        }

        SymbolTreeNode treeNode = parser.TableTree.GetNode(holder.Type.TypeDeclaration.TableTreePath)!;

        MemberInvoke? member = null;
        if (parser.tokens.Check(1)!.Value.Kind == SyntaxKind.OpenParentheseToken) 
        {
            FunctionInvoke? funcInv = await VisitFuncInvokeAsync(parser, treeNode);

            if (funcInv != null) 
            {
                member = new FunctionMemberInvoke(funcInv.Function.Name, funcInv.Function, 
                    funcInv.PassedVals, parser.tokens.Index);
            }
        }
        else 
        {
            string fldName = parser.tokens.Current.Metadata.Raw;

            Symbol? symbol_ = treeNode.Table!.GetSymbol(fldName, SymbolType.Field);
            if (symbol_ == null) 
            {
                await parser.diagnostics.ReportUnknownSymbol(parser.tokens.Current.Metadata.Line, fldName);
                return null;
            }
            parser.tokens.Increment();
            member = new FieldInvoke(fldName, (FieldDeclaration)symbol_.Value.Node, parser.tokens.Index);
        }
        bool last = parser.tokens.Current.Kind != SyntaxKind.DotToken;
        if (!last) parser.tokens.Increment();

        return new MemberExpressionNode(
            member!, 
            last ? null : await visitMemberExp(parser, member?.Holder!), 
            parser.tokens.Index);
    }
}