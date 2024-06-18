using System.Diagnostics.CodeAnalysis;

namespace Pdcl.Core.Diagnostics;
internal struct Warning : IDiagnostic 
{
    public string Description {get; init;}
    public int Identifier {get; init;}
    public int Line {get; init;}

    public override string ToString()
    {
        return $"WARNING ({Identifier}) at line {Line}: \n\t{Description}";
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Identifier, Line);
    }
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Warning warning && GetHashCode() == warning.GetHashCode();
    }
}  