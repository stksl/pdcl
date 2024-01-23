using Pdcl.Core.Syntax;

namespace Pdcl.Core.Preproc;

/// <summary>
/// Preprocessoring context, contains various tables during preproc phase.
/// </summary>
public sealed class PreprocContext 
{
    public readonly MacroBag Macros;
    public PreprocContext()
    {
        Macros = new MacroBag(null);
    }
}
