using System.Reflection.Metadata;

namespace Pdcl.Core.Syntax;

public interface IMemberNode 
{
    MemberModifiers Modifiers {get;}
}

[Flags]
public enum MemberModifiers 
{
    Public = 1,
    Private = 1 << 1,
    Family = 1 << 2,
    Internal = 1 << 3,

    Static = 1 << 4,
}