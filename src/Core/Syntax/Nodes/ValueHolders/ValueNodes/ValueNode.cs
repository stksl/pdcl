
using System.Diagnostics.CodeAnalysis;

namespace Pdcl.Core.Syntax;

/// <summary>
/// A value holder node (either a local var or a field)
/// </summary>
public abstract class ValueNode : SyntaxNode
{
    public readonly TypeNode Type;
    public ValueNode(TypeNode type) 
    {
        Type = type;
    }
}