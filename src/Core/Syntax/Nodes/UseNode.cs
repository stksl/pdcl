
namespace Pdcl.Core.Syntax;

public sealed class UseNode : SyntaxNode 
{
    public readonly string Namespace;
    public string[] NamespacePath => Namespace.Split('.');
    public UseNode(string namespace_, int tokenIndex) : base(tokenIndex)
    {
        Namespace = namespace_;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
}