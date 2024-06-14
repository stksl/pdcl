
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;
public class Macro : IDirective
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
