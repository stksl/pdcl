namespace Pdcl.Core.Syntax;

public abstract class SyntaxNode 
{
    public SyntaxNode()
    {
        
    }
    public abstract IEnumerable<SyntaxNode> GetChildren();

}