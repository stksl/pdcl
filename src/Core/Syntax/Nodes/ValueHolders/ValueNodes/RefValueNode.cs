
namespace Pdcl.Core.Syntax;

/// <summary>
/// Reference to a variable (to get/set the value)
/// </summary>
public sealed class RefValueNode : ValueNode 
{
    public readonly ValueHolderNode Holder;
    public RefValueNode(ValueHolderNode holder) : base(holder.Type)
    {
        Holder = holder;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
}