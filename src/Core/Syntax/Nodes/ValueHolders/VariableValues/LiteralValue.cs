namespace Pdcl.Core.Syntax;

public sealed class LiteralValue : ValueNode
{
    public readonly SyntaxToken LiteralToken;
    public readonly LiteralType Type;

    public LiteralValue(SyntaxToken token, LiteralType type, int tokenInd) : base(tokenInd)
    {
        LiteralToken = token;
        Type = type;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
    public override SyntaxKind GetKind()
    {
        return LiteralToken.Kind;
    }
    public enum LiteralType
    {
        Number,
        String,
        Character
    }
}