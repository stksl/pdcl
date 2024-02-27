namespace Pdcl.Core.Syntax;

public sealed class BinaryOperator : OperatorNode
{
    public readonly BinaryOperators OperatorType;
    public BinaryOperator(BinaryOperators op, int tokenInd) : base(tokenInd)
    {
        OperatorType = op;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
    public enum BinaryOperators
    {
        Plus,
        Minus,
        Multiply,
        Divide,
        Modulo,
        BitwiseShiftLeft,
        BitwiseShiftRight,
        BitwiseOr,
        ShortOr,
        BitwiseAnd,
        ShortAnd,
        BitwiseXor,
        IsEqual,
        IsNotEqual,
        Equals,

    }
}