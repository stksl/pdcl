using System.Collections.Immutable;
using Pdcl.Core.Syntax;
using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;

/// <summary>
/// Preprocessoring context, contains various tables during preproc phase.
/// </summary>
public sealed class PreprocContext 
{
    public readonly MacroBag<Macro> DefinedMacros;
    public readonly MacroBag<NonDefinedMacro> NonDefMacros;
    /// <summary>
    /// Value - subtitution TextPosition and substitution string
    /// </summary>
    public PreprocContext()
    {
        DefinedMacros = new MacroBag<Macro>(null);
        NonDefMacros = new MacroBag<NonDefinedMacro>(null);
    }
}
