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
        switch(parser.tokens.Current.Kind) 
        {
            case > 0 when checkPrefixUnary && SyntaxFacts.IsUnaryOperator(parser.tokens.Current.Kind, out _): // prefix unary op
                result = await VisitorFactory.GetVisitorFor<UnaryExpression>(parser.context)!.VisitAsync(parser);
                break;
            case > 0 when SyntaxFacts.IsLiteralKind(parser.tokens.Current.Kind):
                result = await VisitLiteralAsync(parser);
            break;
            case SyntaxKind.OpenParentheseToken:
                result = await VisitorFactory.GetVisitorFor<ParenthesizedExpression>(parser.context)!.VisitAsync(parser);
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
        
        if (result != null && checkOtherExp) 
        {
            if (SyntaxFacts.IsBinaryOperator(parser.tokens.Current.Kind, out _)) 
            {
                var visitor = 
                    (IExpressionVisitor<BinaryExpression>)VisitorFactory.GetVisitorFor<BinaryExpression>(parser.context)!;
                result = await visitor.VisitAsync(parser, result);
            }
            else if (SyntaxFacts.IsUnaryOperator(parser.tokens.Current.Kind, out _)) 
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
        switch(parser.tokens.Current.Kind) 
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
                ulong mdata = parser.tokens.Current.Metadata.AdditionalMetadata;

                bool isUnsigned = (mdata & (1 << 4)) == (1 << 4);
                bool isLong = (mdata & (1 << 1)) == (1 << 1);
                bool isSingle = (mdata & (1 << 2)) == (1 << 2);
                bool isDouble = (mdata & (1 << 3)) == (1 << 3);
                bool isInteger = (mdata & 1) == 1;

                if (isUnsigned) 
                {
                    if (isSingle || isDouble) 
                    {
                        await parser.diagnostics.ReportUnkownOperandSyntax(parser.tokens.Current.Metadata.Line, 
                            parser.tokens.Current.Metadata.Raw);
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
        LiteralValue res = new LiteralValue(parser.tokens.Current, new PrimitiveTypeNode(pType));
        parser.tokens.Increment();
        return res;
    }
    private async Task<RefValueNode?> VisitRefAsync(Parser parser)
    {
        string name = parser.tokens.Current.Metadata.Raw;

        Symbol? symbol = parser.GetCurrentTableNode().Table!.GetSymbol(name, SymbolType.Variable) ??
            parser.GetCurrentTableNode().Table!.GetSymbol(name, SymbolType.Field);
        if (!symbol.HasValue)
        {
            await parser.diagnostics.ReportUnknownSymbol(parser.tokens.Current.Metadata.Line, name);
            return null;
        }
        parser.tokens.Increment();

        return new RefValueNode((ValueHolderNode)symbol.Value.Node);
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
            ValueNode? passedVal = await VisitorFactory.GetVisitorFor<ValueNode>(parser.context)!.VisitAsync(parser);
            if (passedVal != null) passedVals.Add(passedVal);

            if (parser.tokens.Current.Kind != SyntaxKind.CommaToken && parser.tokens.Current.Kind != SyntaxKind.CloseParentheseToken)
            {
                await parser.diagnostics.ReportNoArgSeparator(parser.tokens.Current.Metadata.Line);
            }

            index++;
        }
        parser.tokens.Increment();
        return new FunctionInvoke((FunctionDeclaration)function.Value.Node, passedVals.ToImmutableArray());
    }
    private async Task<MemberExpression?> VisitMemberExpAsync(Parser parser) 
    {
        RefValueNode? refNode = await VisitRefAsync(parser);

        parser.tokens.Increment(); // skipping '.'

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
        if (!SyntaxHelper.CheckTokens(parser, SyntaxKind.TextToken)) 
        {
            await parser.diagnostics.ReportUnsuitableSyntaxToken(parser.tokens.Current.Metadata.Line, 
                parser.tokens.Current, SyntaxKind.TextToken);
            return null;
        }

        SymbolTreeNode treeNode = parser.AssemblyInfo.TableTree.GetNode(holder.Type.TypeDeclaration.TableTreePath)!;

        MemberInvoke? member = null;
        if (parser.tokens.Check(1)!.Value.Kind == SyntaxKind.OpenParentheseToken) 
        {
            FunctionInvoke? funcInv = await VisitFuncInvokeAsync(parser, treeNode);

            if (funcInv != null) 
            {
                member = new FunctionMemberInvoke(funcInv.Function.Name, funcInv.Function, funcInv.PassedVals);
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
            member = new FieldInvoke(fldName, (FieldDeclaration)symbol_.Value.Node);
        }
        bool last = parser.tokens.Current.Kind != SyntaxKind.DotToken;
        if (!last) parser.tokens.Increment();

        return new MemberExpressionNode(member!, last ? null : await visitMemberExp(parser, member?.Holder!));
    }
}