namespace Pdcl.Core.Syntax;

public class UnaryExpression : ExpressionNode 
{
    public readonly UnaryOperator Operator;
    public readonly ValueNode Operand;
    public UnaryExpression(UnaryOperator op, ValueNode operand, TypeNode resultType) 
        : base(resultType)
    {
        Operator = op;
        Operand = operand;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Operator;
        yield return Operand;
    }
}