
using System.Collections.Immutable;

namespace Pdcl.Core.Syntax;

public sealed class TypeDeclarationNode : SyntaxNode
{
    public readonly ImmutableArray<FunctionMemberDeclaration> Functions;
    public readonly ImmutableArray<FieldDeclaration> Fields;
    public readonly string Name;
    public TypeDeclarationNode(string name, ImmutableArray<FunctionMemberDeclaration> functions, ImmutableArray<FieldDeclaration> fields)
    {
        Name = name;

        Functions = functions;
        Fields = fields;
    }

    public IEnumerable<OperatorOverload> GetOperatorOverloads() 
    {
        yield return null!;
        /* foreach(var func in Functions) 
        {
            if (func is OperatorOverload func_op) yield return func_op;
        } */
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        foreach(var func in Functions) yield return func;
        foreach(var field in Fields) yield return field; 
    }
}