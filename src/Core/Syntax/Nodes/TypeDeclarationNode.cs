
using System.Collections.Immutable;

namespace Pdcl.Core.Syntax;

public sealed class TypeDeclarationNode : SyntaxNode, ISymboled
{
    public readonly ImmutableArray<FunctionDeclaration> Functions;
    public readonly ImmutableArray<FieldDeclaration> Fields;
    public readonly string Name;
    public string TableTreePath {get; private set;}
    public TypeDeclarationNode(string name, ImmutableArray<FunctionDeclaration> functions, ImmutableArray<FieldDeclaration> fields,
        string tableTreePath)
    {
        Name = name;
        TableTreePath = tableTreePath;

        Functions = functions;
        Fields = fields;
    }

    public IEnumerable<OperatorOverload> GetOperatorOverloads() 
    {
        foreach(var func in Functions) 
        {
            if (func is OperatorOverload func_op) yield return func_op;
        }
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        foreach(var func in Functions) yield return func;
        foreach(var field in Fields) yield return field; 
    }
}