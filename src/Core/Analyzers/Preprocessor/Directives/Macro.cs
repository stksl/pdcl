
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;
public interface IMacro 
{
}
/// <summary>
/// A macro that was used in code (non-defined)
/// </summary>
public readonly struct NonDefinedMacro : IMacro 
{
    public readonly TextPosition Position;
    public readonly string Substitution;
    public NonDefinedMacro(TextPosition pos, string sub)
    {
        Position = pos;
        Substitution = sub;
    }
    public override int GetHashCode() 
    {
        return Position.Position;
    }
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is NonDefinedMacro other && other.Position.Equals(Position);
    }
}

public class Macro : IDirective, IMacro
{
    public string Name {get; private set;}
    public TextPosition Position {get; private set;}
    public readonly string Substitution;
    public Macro(string name, string substitution, TextPosition pos)
    {
        Name = name;
        Substitution = substitution;
        Position = pos;
    }
    public override bool Equals(object? obj)
    {
        return obj is Macro m && m.Name == Name;
    }
    public override int GetHashCode()
    {
        return Name.GetHashCode();
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
