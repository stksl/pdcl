namespace Pdcl.Core.Syntax;

public sealed class UnaryOperator : OperatorNode
{
    public readonly UnaryOperators OperatorType;
    public UnaryOperator(UnaryOperators op, int tokenInd) : base(tokenInd)
    {
        OperatorType = op;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
    public enum UnaryOperators
    {
        Increment,
        Decrement,
        Not,
    }
}