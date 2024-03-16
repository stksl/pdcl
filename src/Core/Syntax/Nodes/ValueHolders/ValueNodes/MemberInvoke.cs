
using System.Collections.Immutable;

namespace Pdcl.Core.Syntax;

public abstract class MemberInvoke : ValueNode 
{
    public readonly string Name;
    public readonly ValueHolderNode Holder;
    public MemberInvoke(string name, ValueHolderNode holder) : base(holder.Type)
    {
        Name = name;
        Holder = holder;
    }


}
public sealed class FieldInvoke : MemberInvoke 
{
    public FieldDeclaration Field => (FieldDeclaration)Holder;
    public FieldInvoke(string name, FieldDeclaration field) : base(name, field)
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
    public FunctionMemberInvoke(string name, FunctionDeclaration function, ImmutableArray<ValueNode> passedVals) 
        : base(name, function)
    {
        PassedVals = passedVals;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Function;
    }
}