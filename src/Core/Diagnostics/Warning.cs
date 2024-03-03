using System.Diagnostics.CodeAnalysis;

namespace Pdcl.Core.Diagnostics;
public struct Warning : IDiagnostic 
{
    public string Description => description;
    private readonly string description;
    public int Identifier => identifier;
    private readonly int identifier;
    public int Line => line;
    private readonly int line;
    internal Warning(int ident, string descr, int line_) 
    {
        description = descr;
        identifier = ident;
        line = line_;
    }

    public override string ToString()
    {
        return $"WARNING({Identifier}) at line {line}: \n\t{description}";
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(identifier, line);
    }
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Warning warning && warning.identifier == identifier && warning.line == line;
    }
}  