namespace Pdcl.Core.Syntax;

public sealed class AssignExpression : BinaryExpression 
{
    public AssignExpression(RefValueNode left, ValueNode right) 
        : base(new BinaryOperator(BinaryOperator.BinaryOperators.Equals), left, right, left.Type)
    {
    }
}