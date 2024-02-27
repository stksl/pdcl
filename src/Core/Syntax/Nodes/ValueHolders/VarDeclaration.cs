
namespace Pdcl.Core.Syntax;

public sealed class VarDeclaration : ValueHolderNode
{
    public VarDeclaration(string name, TypeNode type, int tokenInd)
        : base(name, type, hasGetter: true, hasSetter: true, tokenInd)
    {
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Type;
    }
}