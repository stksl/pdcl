namespace Pdcl.Core.Syntax;

public sealed class LiteralValue : VariableValue 
{
    public readonly SyntaxToken LiteralToken;
    public readonly LiteralType Type;

    public LiteralValue(int tokenInd) : base(tokenInd)
    {
        
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
    public enum LiteralType 
    {
        Number,
        String,
        Character
    }
}