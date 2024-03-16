
namespace Pdcl.Core.Syntax;

public sealed class FieldDeclaration : ValueHolderNode, IMemberNode 
{
    public MemberModifiers Modifiers {get; private set;}
    public FieldDeclaration(string name, TypeNode type, MemberModifiers mods, bool hasGetter, bool hasSetter) 
        : base(name, type, hasGetter, hasSetter)
    {
        Modifiers = mods;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Type;
    }
}