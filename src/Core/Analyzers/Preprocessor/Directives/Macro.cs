
using System.Collections.Immutable;
using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;

public class Macro : IDirective 
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
}
public sealed class ArgumentedMacro : Macro 
{
    public readonly ImmutableArray<string> Args;
    public ArgumentedMacro(string name, string substitution, IList<string> args, TextPosition pos) : base(name, substitution, pos)
    {
        Args = args.ToImmutableArray();
    }
}
