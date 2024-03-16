namespace Pdcl.Core.Syntax;

public sealed class OperatorOverload : FunctionMemberDeclaration 
{
    public readonly OperatorNode Operator;
    public OperatorOverload(OperatorNode operator_, FunctionSignature sig, FunctionBody body, MemberModifiers mods, string tableTreePath) 
        : base(sig, body, mods, tableTreePath)
    {

        Operator = operator_;
    }
}