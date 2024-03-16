
namespace Pdcl.Core.Syntax;

public sealed class NamespaceNode : SyntaxNode 
{
    public readonly string FullName;
    public string[] NameParts => FullName.Split('.');

    private List<SyntaxNode> globalNodes;
    public NamespaceNode(string fullName)
    {
        FullName = fullName;

        globalNodes = new List<SyntaxNode>();
    }

    public void addChild(SyntaxNode node) 
    {
        globalNodes.Add(node);
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return globalNodes;
    }

}