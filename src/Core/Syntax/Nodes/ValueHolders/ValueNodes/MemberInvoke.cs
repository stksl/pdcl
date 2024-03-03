
using System.Collections.Immutable;

namespace Pdcl.Core.Syntax;

public abstract class MemberInvoke : ValueNode 
{
    public readonly string Name;
    public readonly ValueHolderNode Holder;
    public MemberInvoke(string name, ValueHolderNode holder, int tokenInd) : base(tokenInd)
    {
        Name = name;
        Holder = holder;
    }


}
public sealed class FieldInvoke : MemberInvoke 
{
    public FieldDeclaration Field => (FieldDeclaration)Holder;
    public FieldInvoke(string name, FieldDeclaration field, int tokenInd) : base(name, field, tokenInd)
    {
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Field;
    }
}
public sealed class FunctionMemberInvoke : MemberInvoke 
{
    public FunctionDeclaration Function => (FunctionDeclaration)Holder;
    public readonly ImmutableArray<ValueNode> PassedVals;
    public FunctionMemberInvoke(string name, FunctionDeclaration function, ImmutableArray<ValueNode> passedVals, int tokenInd) 
        : base(name, function, tokenInd)
    {
        PassedVals = passedVals;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Function;
    }
}