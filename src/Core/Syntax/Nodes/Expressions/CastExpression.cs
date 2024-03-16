namespace Pdcl.Core.Syntax;

public sealed class CastExpression : UnaryExpression 
{
    public readonly TypeNode CastTo;
    public CastExpression(ValueNode operand_, TypeNode castTo) 
    : base(new UnaryOperator(UnaryOperator.UnaryOperators.Cast, isPrefix: true), operand_, castTo)
    {
        CastTo = castTo;
    }
}