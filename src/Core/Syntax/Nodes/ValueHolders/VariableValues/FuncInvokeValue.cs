
using System.Collections.Immutable;

namespace Pdcl.Core.Syntax;

public sealed class FuncInvokeValue : ValueNode 
{
    public readonly FunctionDeclaration Function;
    public readonly ImmutableArray<ValueNode> PassedVals;
    public FuncInvokeValue(FunctionDeclaration function, ImmutableArray<ValueNode> passedVals, int tokenInd) 
        : base(tokenInd)
    {
        Function = function;
        PassedVals = passedVals;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Function;
    }
    public override SyntaxKind GetKind()
    {
        return SyntaxKind.TextToken;
    }
}