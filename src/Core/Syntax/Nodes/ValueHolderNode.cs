
namespace Pdcl.Core.Syntax;

/// <summary>
/// Something that can be either a left or/and a right value
/// </summary>
public abstract class ValueHolderNode : SyntaxNode 
{
    public bool HasGetter {get; init;}
    public bool HasSetter {get; init;}
    public readonly string Name;
    public readonly TypeNode Type;
    public ValueHolderNode(string name, TypeNode type)
    {
        Name = name;
        Type = type;
    }
}