
using System.Collections.Immutable;

namespace Pdcl.Core.Syntax;

public sealed class TypeDeclarationNode : SyntaxNode, ISymboled
{
    public readonly ImmutableArray<FunctionDeclaration> Functions;
    public readonly ImmutableArray<FieldDeclaration> Fields;

    public string TableTreePath {get; private set;}
    public TypeDeclarationNode(ImmutableArray<FunctionDeclaration> functions, ImmutableArray<FieldDeclaration> fields,
        string tableTreePath, int tokenInd)
        : base(tokenInd)
    {
        TableTreePath = tableTreePath;

        Functions = functions;
        Fields = fields;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        foreach(var func in Functions) yield return func;
        foreach(var field in Fields) yield return field; 
    }
}