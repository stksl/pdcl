namespace Pdcl.Core.Syntax;

public sealed class OperatorOverload : FunctionDeclaration 
{
    public readonly OperatorNode Operator;
    public OperatorOverload(OperatorNode operator_, FunctionSignature sig, FunctionBody body, AccessModifiers mods, string tableTreePath) 
        : base(sig, body, tableTreePath)
    {

        Operator = operator_;
    }
}