namespace Pdcl.Core.Syntax;

public sealed class UnaryOperator : OperatorNode
{
    public readonly bool IsPrefix;
    public readonly UnaryOperators OperatorType;
    public UnaryOperator(UnaryOperators op, bool isPrefix)
    {
        IsPrefix = isPrefix;
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
        Minus,

        Cast,
        Implicit,
    }
}