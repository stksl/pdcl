using System.Collections.Immutable;

namespace Pdcl.Core.Syntax;

public sealed class FunctionDeclaration : ValueHolderNode, ISymboled
{
    public readonly FunctionSignature Signature;
    public readonly FunctionBody Body;

    public string TableTreePath {get; private set;}
    public FunctionDeclaration(FunctionSignature sig, FunctionBody body, string tableTreePath, int tokenInd) 
        : base(sig.Name, sig.ReturnType, hasGetter: true, hasSetter: false, tokenInd)
    {
        TableTreePath = tableTreePath;

        Signature = sig;
        Body = body;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Signature;
        yield return Body;
    }
}
public sealed class FunctionSignature : SyntaxNode 
{
    public readonly string Name;
    public readonly TypeNode ReturnType;
    public readonly ImmutableDictionary<string, TypeNode> Arguments;
    public FunctionSignature(string name, TypeNode retType, ImmutableDictionary<string, TypeNode> args, int tokenInd) 
        : base(tokenInd)
    {
        Name = name;
        ReturnType = retType;
        Arguments = args;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return ReturnType;
    }
}
public sealed class FunctionBody : SyntaxNode 
{
    public FunctionBody() : base(0)
    {
        throw new Exception();
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        throw new NotImplementedException();
    }
}