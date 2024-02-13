using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;

/// <summary>
/// A directive that may possibly contain child directives 
/// </summary>
public abstract class ComplexDirective : IDirective
{
    protected ComplexDirective(string name, TextPosition pos) : base(name, pos)
    {

    }
    /// <summary>
    /// Gets child directives in the directive
    /// </summary>
    /// <returns></returns>
    public abstract IList<IDirective> GetChildren();
}