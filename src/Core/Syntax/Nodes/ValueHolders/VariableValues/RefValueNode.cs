
namespace Pdcl.Core.Syntax;

/// <summary>
/// Reference to a variable (to get/set the value)
/// </summary>
public sealed class RefValueNode : ValueNode 
{
    public readonly ValueHolderNode Holder;
    public RefValueNode(ValueHolderNode holder, int tokenInd) : base(tokenInd)
    {
        Holder = holder;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
    public override SyntaxKind GetKind()
    {
        return SyntaxKind.TextToken;
    }
}