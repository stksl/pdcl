namespace Pdcl.Core.Syntax;

public sealed class ParenthesizedExpression : ExpressionNode 
{
    public readonly VariableValue Value;
    public ParenthesizedExpression(VariableValue value, int tokenInd) : base(tokenInd)
    {
        Value = value;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Value;
    }
}