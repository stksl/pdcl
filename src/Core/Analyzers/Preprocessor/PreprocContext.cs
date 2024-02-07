using System.Collections.Immutable;
using Pdcl.Core.Syntax;
using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;

/// <summary>
/// Preprocessoring context, contains various tables during preproc phase.
/// </summary>
public sealed class PreprocContext 
{
    public readonly Bag<IDirective> Directives;
    public readonly Bag<NonDefinedMacro> NonDefMacros;
    /// <summary>
    /// Value - subtitution TextPosition and substitution string
    /// </summary>
    public PreprocContext()
    {
        NonDefMacros = new Bag<NonDefinedMacro>();
        Directives = new Bag<IDirective>();
    }

    public void AddDirective(IDirective directive) 
    {
        Directives.Insert(directive);
    }
    public IDirective? GetDirective(int hashcode) 
    {
        return Directives.Get(hashcode);
    }
    public void PrepareForNewFile() 
    {
        Directives.AddLast();
        NonDefMacros.AddLast();
    }
}
