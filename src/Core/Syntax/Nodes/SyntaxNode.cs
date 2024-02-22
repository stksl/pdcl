namespace Pdcl.Core.Syntax;

public abstract class SyntaxNode 
{
    private readonly int tokenIndex;
    public SyntaxNode(int tokenIndex_)
    {
        // used for hash to perform equality checks
        tokenIndex = tokenIndex_;
    }
    public abstract IEnumerable<SyntaxNode> GetChildren();

    public override bool Equals(object? obj)
    {
        return obj is SyntaxNode node && node.GetHashCode() == GetHashCode();
    }
    public override int GetHashCode()
    {
        return HashCode.Combine<string, int>(GetType().Name, tokenIndex);
    }
    public static bool operator ==(SyntaxNode? left, SyntaxNode? right) 
    {
        return left is not null && left.Equals(right);    
    }   
    public static bool operator !=(SyntaxNode? left, SyntaxNode? right) 
    {
        return left is not null && !left.Equals(right);
    }
}