
using System.Collections.Immutable;

namespace Pdcl.Core.Preproc;

public class Macro : IDirective 
{
    public string Name {get; private set;}
    public readonly string Substitution;
    public Macro(string name, string substitution)
    {
        Name = name;
        Substitution = substitution;
    }
}
public sealed class ArgumentedMacro : Macro 
{
    public readonly ImmutableArray<string> Args;
    public ArgumentedMacro(string name, string substitution, IList<string> args) : base(name, substitution)
    {
        Args = args.ToImmutableArray();
    }
}
