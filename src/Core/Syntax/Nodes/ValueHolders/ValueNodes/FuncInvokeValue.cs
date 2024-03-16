
using System.Collections.Immutable;

namespace Pdcl.Core.Syntax;

public class FunctionInvoke : ValueNode 
{
    public readonly FunctionDeclaration Function;
    public readonly ImmutableArray<ValueNode> PassedVals;
    public FunctionInvoke(FunctionDeclaration function, ImmutableArray<ValueNode> passedVals) 
        : base(function.Signature.ReturnType)
    {
        Function = function;
        PassedVals = passedVals;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Function;
    }
}