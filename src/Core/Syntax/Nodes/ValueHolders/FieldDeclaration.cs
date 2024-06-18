
namespace Pdcl.Core.Syntax;

public sealed class FieldDeclaration : MemberNode 
{
    public FieldDeclaration(string name, TypeNode type, AccessModifiers mods) 
        : base(mods, name, type)
    {
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Type;
    }
}