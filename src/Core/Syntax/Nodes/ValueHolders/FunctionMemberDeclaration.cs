namespace Pdcl.Core.Syntax;

public class FunctionMemberDeclaration : FunctionDeclaration, IMemberNode
{
    public MemberModifiers Modifiers {get; private set;}
    public FunctionMemberDeclaration(FunctionSignature sig, FunctionBody body, MemberModifiers mods, string tableTreePath)
        : base(sig, body, tableTreePath)
    {
        Modifiers = mods;
    }
}