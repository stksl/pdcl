namespace Pdcl.Core.Syntax;

public class BinaryExpression : ExpressionNode 
{
    public readonly BinaryOperator Operator;
    public readonly ValueNode Left;
    public readonly ValueNode Right;
    public BinaryExpression(BinaryOperator op, ValueNode left, ValueNode right, TypeNode resultType) 
        : base(resultType)
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