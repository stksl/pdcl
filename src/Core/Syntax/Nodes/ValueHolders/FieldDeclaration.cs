
namespace Pdcl.Core.Syntax;

public sealed class FieldDeclaration : MemberNode 
{
    public FieldDeclaration(string name, TypeNode type, AccessModifiers mods, bool hasGetter, bool hasSetter) 
        : base(mods, name, type, isStatic: /*temp */ true, hasGetter, hasSetter)
    {
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Type;
    }
}