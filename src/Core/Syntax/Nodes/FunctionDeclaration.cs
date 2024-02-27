using System.Collections.Immutable;

namespace Pdcl.Core.Syntax;

public sealed class FunctionDeclaration : SyntaxNode 
{
    public readonly FunctionSignature Signature;
    public readonly FunctionBody Body;
    public FunctionDeclaration(FunctionSignature sig, FunctionBody body, int tokenInd) : base(tokenInd)
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