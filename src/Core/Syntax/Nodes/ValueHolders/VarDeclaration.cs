
namespace Pdcl.Core.Syntax;

public sealed class VarDeclaration : ValueHolderNode
{
    public VarDeclaration(string name, TypeNode type)
        : base(name, type, hasGetter: true, hasSetter: true)
    {
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Type;
    }
}