using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Pdcl.Core.Diagnostics;

public struct Error : IDiagnostic
{
    public string Description => description;
    private readonly string description;
    public int Identifier => identifier;
    private readonly int identifier;
    public int Line => line;
    private readonly int line;
    internal Error(int ident, string descr, int line_) 
    {
        description = descr;
        identifier = ident;
        line = line_;
    }
    public override string ToString()
    {
        return $"ERROR({identifier}) at line {line}: {description}";
    }
    public override int GetHashCode() 
    {
        return HashCode.Combine(identifier, line);
    }
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Error error && error.identifier == identifier && error.line == line;
    }
}