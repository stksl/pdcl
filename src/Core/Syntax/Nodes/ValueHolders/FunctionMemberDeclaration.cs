
namespace Pdcl.Core.Syntax;

public sealed class FunctionMemberDeclaration : MemberNode, ISymboled
{
    public readonly FunctionDeclaration Function;
    public string TableTreePath {get; private set;}
    public FunctionMemberDeclaration(FunctionDeclaration function, AccessModifiers mods, string tableTreePath)
        : base(mods, function.Name, function.Signature.ReturnType, hasGetter: true, hasSetter: false)
    {
        Function = function;
        TableTreePath = tableTreePath;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Function;
    }
}