using System.Reflection.Metadata;

namespace Pdcl.Core.Syntax;

public abstract class MemberNode : ValueHolderNode 
{
    public readonly AccessModifiers AccessModifiers;
    public readonly bool IsStatic;
    public MemberNode(AccessModifiers mods, string name, TypeNode type, bool isStatic, bool hasGetter, bool hasSetter) 
    : base(name, type, hasGetter, hasSetter)
    {
        AccessModifiers = mods;
        IsStatic = isStatic;
    }
}

[Flags]
public enum AccessModifiers 
{
    Public = 1,
    Private = 1 << 1,
    Family = 1 << 2,
    Assembly = 1 << 3,
}