using System.Collections.Immutable;

namespace Pdcl.Core.Syntax;

public class FunctionDeclaration : ValueHolderNode, ISymboled
{
    public readonly FunctionSignature Signature;
    public readonly FunctionBody Body;

    public string TableTreePath {get; private set;}
    public FunctionDeclaration(FunctionSignature sig, FunctionBody body, string tableTreePath) 
        : base(sig.Name, sig.ReturnType, hasGetter: true, hasSetter: false)
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
    public FunctionSignature(string name, TypeNode retType, ImmutableDictionary<string, TypeNode> args) 
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
    public readonly LinkedListNode<BodyEnclosedNode> Head;
    public FunctionBody(LinkedListNode<BodyEnclosedNode> head)
    {
        Head = head;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        var runner = Head;
        while (runner != null) 
        {
            yield return runner.Value;
            runner = runner.Next;
        }
    }
}