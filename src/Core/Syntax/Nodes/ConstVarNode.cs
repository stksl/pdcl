
namespace Pdcl.Core.Syntax;

public sealed class ConstVarNode : ValueHolderNode
{
    public LiteralValue ConstValue {get; init;}
    public ConstVarNode(string name, TypeNode type)
        : base(name, type)
    {
        HasGetter = true;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Type;
        yield return ConstValue;
    }
}