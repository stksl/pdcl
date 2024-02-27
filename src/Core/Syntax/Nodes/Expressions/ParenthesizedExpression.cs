namespace Pdcl.Core.Syntax;

public sealed class ParenthesizedExpression : ExpressionNode 
{
    public readonly ValueNode Value;
    public ParenthesizedExpression(ValueNode value, int tokenInd) : base(tokenInd)
    {
        Value = value;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Value;
    }

    public override SyntaxKind GetKind()
    {
        return SyntaxKind.OpenParentheseToken;
    }
}