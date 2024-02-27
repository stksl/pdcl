namespace Pdcl.Core.Syntax;

public sealed class UnaryExpression : ExpressionNode 
{
    public readonly UnaryOperator Operator;
    public readonly VariableValue Operand;
    public UnaryExpression(UnaryOperator op, VariableValue operand, int tokenInd) : base(tokenInd)
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