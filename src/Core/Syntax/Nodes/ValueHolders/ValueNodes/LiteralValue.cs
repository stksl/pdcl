namespace Pdcl.Core.Syntax;

public sealed class LiteralValue : ValueNode
{
    public readonly SyntaxToken LiteralToken;
    public new PrimitiveTypeNode Type => (PrimitiveTypeNode)base.Type;
    public LiteralValue(SyntaxToken token, PrimitiveTypeNode type) : base(type)
    {
        LiteralToken = token;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
}