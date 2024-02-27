
namespace Pdcl.Core.Syntax;

/// <summary>
/// Something that can be either a left or/and a right value
/// </summary>
public abstract class ValueHolderNode : SyntaxNode 
{
    public readonly bool HasGetter;
    public readonly bool HasSetter;
    public readonly string Name;
    public readonly TypeNode Type;
    public ValueHolderNode(string name, TypeNode type, bool hasGetter, bool hasSetter, int tokenInd) : base(tokenInd)
    {
        Name = name;
        Type = type;

        HasGetter = hasGetter;
        HasSetter = hasSetter;
    }
}