using System.Reflection.Metadata;

namespace Pdcl.Core.Syntax;

public abstract class MemberNode : ValueHolderNode 
{
    public readonly AccessModifiers AccessModifiers;
    public bool IsStatic {get; init;}
    public MemberNode(AccessModifiers mods, string name, TypeNode type) 
    : base(name, type)
    {
        AccessModifiers = mods;
    }
}

[Flags]
public enum AccessModifiers 
{
    Public = 1,
    Private = 1 << 1,
    Assembly = 1 << 3,
}