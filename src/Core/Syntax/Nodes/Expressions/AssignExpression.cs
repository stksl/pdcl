namespace Pdcl.Core.Syntax;

public sealed class AssignExpression : BinaryExpression 
{
    public AssignExpression(RefValueNode left, ValueNode right, int tokenInd) 
        : base(new BinaryOperator(BinaryOperator.BinaryOperators.Equals, tokenInd), left, right, tokenInd)
    {
    }
}