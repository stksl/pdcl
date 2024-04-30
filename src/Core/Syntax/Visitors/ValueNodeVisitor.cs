using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
using System.Xml;

namespace Pdcl.Core.Syntax;

internal sealed partial class ValueNodeVisitor : IVisitor<ValueNode>
{
    public static ValueNodeVisitor Instance => _instance;
    private static ValueNodeVisitor _instance = new();
    private ValueNodeVisitor() {} 
    public Task<ValueNode?> VisitAsync(Parser parser)
    {
        return VisitAsync(parser, checkPrefixUnary: true, checkOtherExp: true);
    }
    // We dont want to check expressions recursivelly when parsing an unary expression
    internal async Task<ValueNode?> VisitAsync(Parser parser, bool checkPrefixUnary, bool checkOtherExp)
    {
        ValueNode? result = null;
        switch(parser.CurrentToken.Kind) 
        {
            case > 0 when checkPrefixUnary && SyntaxFacts.IsUnaryOperator(parser.CurrentToken.Kind, out _): // prefix unary op
                result = await VisitorFactory.GetVisitorFor<UnaryExpression>(parser.context)!.VisitAsync(parser);
                break;
            case > 0 when SyntaxFacts.IsLiteralKind(parser.CurrentToken.Kind):
                result = await VisitLiteralAsync(parser);
            break;
            case SyntaxKind.OpenParentheseToken:
                result = await VisitorFactory.GetVisitorFor<ParenthesizedExpression>(parser.context)!.VisitAsync(parser);
                break;
            case SyntaxKind.TextToken:
                string name = parser.CurrentToken.Metadata.Raw;
                result = parser.ConsumeToken().Kind switch 
                {
                    SyntaxKind.OpenParentheseToken => await VisitFuncInvokeAsync(parser, parser.GetCurrentTableNode(), name),
                    SyntaxKind.DotToken => await VisitMemberExpAsync(parser, name),
                    _ => await VisitRefAsync(parser, name) 
                };
                break;
            default:
                await parser.diagnostics.ReportUnkownOperandSyntax(parser.CurrentToken.Metadata.Line,
                parser.CurrentToken.Metadata.Raw);
                break;
        }
        
        if (result != null && checkOtherExp) 
        {
            if (SyntaxFacts.IsBinaryOperator(parser.CurrentToken.Kind, out _)) 
            {
                var visitor = 
                    (IExpressionVisitor<BinaryExpression>)VisitorFactory.GetVisitorFor<BinaryExpression>(parser.context)!;
                result = await visitor.VisitAsync(parser, result);
            }
            else if (SyntaxFacts.IsUnaryOperator(parser.CurrentToken.Kind, out _)) 
            {
                var visitor = 
                    (IExpressionVisitor<UnaryExpression>)VisitorFactory.GetVisitorFor<UnaryExpression>(parser.context)!;
                result = await visitor.VisitAsync(parser, result);
            }
        }

        return result;
    }
    private async Task<LiteralValue> VisitLiteralAsync(Parser parser)
    {
        PrimitiveTypeNode.PrimitiveTypes pType = default;
        switch(parser.CurrentToken.Kind) 
        {
            case SyntaxKind.CharLiteral:
                pType = PrimitiveTypeNode.PrimitiveTypes.Char;
                break;
            case SyntaxKind.StringLiteral:
                pType = PrimitiveTypeNode.PrimitiveTypes.String;
                break;
            case SyntaxKind.TrueToken:
            case SyntaxKind.FalseToken:
                pType = PrimitiveTypeNode.PrimitiveTypes.Bool;
                break;
            case SyntaxKind.NumberToken:
                ulong mdata = parser.CurrentToken.Metadata.AdditionalMetadata;

                bool isUnsigned = (mdata & (1 << 4)) == (1 << 4);
                bool isLong = (mdata & (1 << 1)) == (1 << 1);
                bool isSingle = (mdata & (1 << 2)) == (1 << 2);
                bool isDouble = (mdata & (1 << 3)) == (1 << 3);
                bool isInteger = (mdata & 1) == 1;

                if (isUnsigned) 
                {
                    if (isSingle || isDouble) 
                    {
                        await parser.diagnostics.ReportUnkownOperandSyntax(parser.CurrentToken.Metadata.Line, 
                            parser.CurrentToken.Metadata.Raw);
                        return null!;
                    }
                    pType = isLong ? PrimitiveTypeNode.PrimitiveTypes.UInt64 : PrimitiveTypeNode.PrimitiveTypes.UInt32;
                }
                else if (isInteger) 
                {
                    pType = isLong ? PrimitiveTypeNode.PrimitiveTypes.Int64 : PrimitiveTypeNode.PrimitiveTypes.Int32;
                }
                else 
                {
                    pType = isDouble ? PrimitiveTypeNode.PrimitiveTypes.Float64 : PrimitiveTypeNode.PrimitiveTypes.Float32;
                }
                break;
        }            
        LiteralValue res = new LiteralValue(parser.CurrentToken, new PrimitiveTypeNode(pType));
        parser.ConsumeToken();
        return res;
    }
    private async Task<RefValueNode?> VisitRefAsync(Parser parser, string name)
    {
        Symbol? symbol = parser.GetCurrentTableNode().Table!.GetSymbol(name, SymbolType.Variable) ??
            parser.GetCurrentTableNode().Table!.GetSymbol(name, SymbolType.Field);
        if (!symbol.HasValue)
        {
            await parser.diagnostics.ReportUnknownSymbol(parser.CurrentToken.Metadata.Line, name);
            return null;
        }

        return new RefValueNode((ValueHolderNode)symbol.Value.Node);
    }
    private async Task<FunctionInvoke?> VisitFuncInvokeAsync(Parser parser, SymbolTreeNode treeNode, string name)
    {
        Symbol? function = treeNode.GetSymbol(name, SymbolType.Function);
        if (!function.HasValue)
        {
            await parser.diagnostics.ReportUnknownSymbol(parser.CurrentToken.Metadata.Line,
                name);
            return null;
        }
        parser.ConsumeToken(SyntaxKind.OpenParentheseToken);
        List<ValueNode> passedVals = new List<ValueNode>();
        int index = 0;
        while (parser.CurrentToken.Kind != SyntaxKind.CloseParentheseToken)
        {
            ValueNode? passedVal = await VisitorFactory.GetVisitorFor<ValueNode>(parser.context)!.VisitAsync(parser);
            if (passedVal != null) passedVals.Add(passedVal);

            if (parser.CurrentToken.Kind == SyntaxKind.CloseParentheseToken)
                parser.ConsumeToken(SyntaxKind.CommaToken);

            index++;
        }
        parser.ConsumeToken(SyntaxKind.CloseParentheseToken);
        return new FunctionInvoke((FunctionDeclaration)function.Value.Node, passedVals.ToImmutableArray());
    }
    private async Task<MemberExpression?> VisitMemberExpAsync(Parser parser, string name) 
    {
        RefValueNode? refNode = await VisitRefAsync(parser, name);

        parser.ConsumeToken(SyntaxKind.DotToken);

        MemberExpressionNode? root = null;
        if (refNode != null) root = await visitMemberExp(parser, refNode.Holder);
        
        MemberExpressionNode? tail = root;
        while (tail?.Next != null) 
        {
            tail = tail.Next;
        } 
        return new MemberExpression(refNode!, root!, tail!);
    }
    private async Task<MemberExpressionNode?> visitMemberExp(Parser parser, ValueHolderNode holder) 
    {
        if (parser.CurrentToken.Kind != SyntaxKind.TextToken) 
        {
            return null;
        }
        string memberName = parser.CurrentToken.Metadata.Raw;
        SymbolTreeNode treeNode = parser.AssemblyInfo.TableTree.GetNode(holder.Type.TypeDeclaration.TableTreePath)!;

        MemberInvoke? member = null;
        if (parser.ConsumeToken().Kind == SyntaxKind.OpenParentheseToken) 
        {
            FunctionInvoke? funcInv = await VisitFuncInvokeAsync(parser, treeNode, memberName);

            if (funcInv != null) 
            {
                member = new FunctionMemberInvoke(funcInv.Function.Name, funcInv.Function, funcInv.PassedVals);
            }
        }
        else 
        {
            Symbol? symbol_ = treeNode.Table!.GetSymbol(memberName, SymbolType.Field);
            if (symbol_ == null) 
            {
                await parser.diagnostics.ReportUnknownSymbol(parser.CurrentToken.Metadata.Line, memberName);
                return null;
            }
            member = new FieldInvoke(memberName, (FieldDeclaration)symbol_.Value.Node);
        }
        bool last = parser.CurrentToken.Kind != SyntaxKind.DotToken;
        if (!last) parser.ConsumeToken();

        return new MemberExpressionNode(member!, last ? null : await visitMemberExp(parser, member?.Holder!));
    }
}