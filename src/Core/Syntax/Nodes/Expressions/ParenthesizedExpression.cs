namespace Pdcl.Core.Syntax;

public sealed class ParenthesizedExpression : ExpressionNode 
{
    public readonly ValueNode Value;
    public ParenthesizedExpression(ValueNode value) : base(value.Type)
    {
        Value = value;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Value;
    }
}