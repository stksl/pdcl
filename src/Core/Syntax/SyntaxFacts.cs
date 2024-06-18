using System.Diagnostics.Contracts;

namespace Pdcl.Core.Syntax;

internal static class SyntaxFacts
{
    public static bool IsLiteralKind(SyntaxKind kind)
    {
        switch (kind)
        {
            case SyntaxKind.NumberToken:
            case SyntaxKind.StringLiteral:
            case SyntaxKind.CharLiteral:
                return true;
            default: return false;
        }
    }
    public static bool IsOperandKind(SyntaxKind kind) => IsLiteralKind(kind) || kind == SyntaxKind.TextToken;

    public static bool IsPrimitiveType(string name, out PrimitiveTypes? type)
    {
        type = null;
        foreach (var primitiveType in Enum.GetValues<PrimitiveTypes>())
        {
            if (primitiveType.ToString().ToLower() == name)
            {
                type = primitiveType;
                return true;
            }
        }
        return false;
    }
    public static AccessModifiers? IsAccessModifier(SyntaxKind kind) 
    {
        switch(kind) 
        {
            case SyntaxKind.PublicToken:
                return AccessModifiers.Public;
            case SyntaxKind.PrivateToken:
                return AccessModifiers.Private;
            case SyntaxKind.AssemblyToken:
                return AccessModifiers.Assembly;
            default:
                return null;
        }
    }
    public static bool IsBinaryOperator(SyntaxKind kind, out BinaryOperator.BinaryOperators? op_)
    {
        op_ = null;
        switch (kind)
        {
            case SyntaxKind.PlusToken:
                op_ = BinaryOperator.BinaryOperators.Plus;
                break;
            case SyntaxKind.MinusToken:
                op_ = BinaryOperator.BinaryOperators.Minus;
                break;
            case SyntaxKind.StarToken:
                op_ = BinaryOperator.BinaryOperators.Multiply;
                break;
            case SyntaxKind.SlashToken:
                op_ = BinaryOperator.BinaryOperators.Divide;
                break;
            case SyntaxKind.PercentToken:
                op_ = BinaryOperator.BinaryOperators.Modulo;
                break;
            case SyntaxKind.LeftShiftToken:
                op_ = BinaryOperator.BinaryOperators.BitwiseShiftLeft;
                break;
            case SyntaxKind.RightShiftToken:
                op_ = BinaryOperator.BinaryOperators.BitwiseShiftRight;
                break;
            case SyntaxKind.PipeToken:
                op_ = BinaryOperator.BinaryOperators.BitwiseOr;
                break;
            case SyntaxKind.ShortOrToken:
                op_ = BinaryOperator.BinaryOperators.ShortOr;
                break;
            case SyntaxKind.AmpersandToken:
                op_ = BinaryOperator.BinaryOperators.BitwiseAnd;
                break;
            case SyntaxKind.ShortAndToken:
                op_ = BinaryOperator.BinaryOperators.ShortAnd;
                break;
            case SyntaxKind.CaretToken:
                op_ = BinaryOperator.BinaryOperators.BitwiseXor;
                break;
            case SyntaxKind.IsEqualToken:
                op_ = BinaryOperator.BinaryOperators.IsEqual;
                break;
            case SyntaxKind.NotEqualToken:
                op_ = BinaryOperator.BinaryOperators.IsNotEqual;
                break;
            case SyntaxKind.EqualToken:
                op_ = BinaryOperator.BinaryOperators.Equals;
                break;
            default: return false;
        }
        return true;
    }
    public static bool IsUnaryOperator(SyntaxKind kind, out UnaryOperator.UnaryOperators? unaryOp) 
    {
        unaryOp = null;
        switch(kind) 
        {
            case SyntaxKind.IncrementToken:
                unaryOp = UnaryOperator.UnaryOperators.Increment;
                break;
            case SyntaxKind.DecrementToken:
                unaryOp = UnaryOperator.UnaryOperators.Decrement;
                break;
            case SyntaxKind.ExclamationToken:
                unaryOp = UnaryOperator.UnaryOperators.Not;
                break;
            case SyntaxKind.MinusToken:
                unaryOp = UnaryOperator.UnaryOperators.Minus;
                break;
            case SyntaxKind.OpenBracketToken:
                unaryOp = UnaryOperator.UnaryOperators.Cast;
                break;
            default: return false;
        }
        return true;
    }
}