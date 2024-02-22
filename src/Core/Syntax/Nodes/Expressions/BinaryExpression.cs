namespace Pdcl.Core.Syntax;

public sealed class BinaryExpression : ExpressionNode 
{
    public readonly BinaryOperator Operator;
    public readonly VariableValue Left;
    public readonly VariableValue Right;
    public BinaryExpression(BinaryOperator op, VariableValue left, VariableValue right, int tokenInd) : base(tokenInd)
    {
        Operator = op;
        Left = left;
        Right = right;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Operator;
        yield return Left;
        yield return Right;
    }
}