
namespace Pdcl.Core.Syntax;

public sealed class FieldDeclaration : ValueHolderNode 
{
    public FieldDeclaration(string name, TypeNode type, bool hasGetter, bool hasSetter, int tokenInd) 
        : base(name, type, hasGetter, hasSetter, tokenInd)
    {
        
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Type;
    }
}