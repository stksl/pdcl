
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;
public interface IMacro 
{
}
/// <summary>
/// A macro that was used in code (non-defined)
/// </summary>
public sealed class NonDefinedMacro : IMacro 
{
    public readonly TextPosition Position;
    public readonly string Substitution;
    public NonDefinedMacro(TextPosition pos, string sub)
    {
        Position = pos;
        Substitution = sub;
    }
    public override bool Equals([NotNullWhen(true)]object? obj)
    {
        return obj is NonDefinedMacro m && m.Position.Position == Position.Position;
    }
    public override int GetHashCode()
    {
        return Position.Position;
    }
}

public class Macro : IDirective, IMacro
{
    public readonly string Substitution;
    public Macro(string name, string substitution, TextPosition pos) : base(name, pos)
    {
        Substitution = substitution;
    }
}
public sealed class ArgumentedMacro : Macro 
{
    public readonly ImmutableArray<string> ArgNames;
    public ArgumentedMacro(string name, string substitutionString, IList<string> args, TextPosition pos) 
        : base(name, substitutionString, pos)
    {
        ArgNames = args.ToImmutableArray();
    }
}
