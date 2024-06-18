
namespace Pdcl.Core.Syntax;

public sealed class FunctionMemberDeclaration : MemberNode
{
    public readonly FunctionSignature Signature;
    public readonly FunctionBody Body;
    public FunctionMemberDeclaration(FunctionSignature sig, FunctionBody body, AccessModifiers mods)
        : base(mods, sig.Name, sig.ReturnType)
    {
        Signature = sig;
        Body = body;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Signature;
        yield return Body;
    }
}