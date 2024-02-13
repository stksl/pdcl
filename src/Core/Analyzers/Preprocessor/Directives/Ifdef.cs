
using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;

public sealed class Ifdef : BranchedDirective 
{
    public Ifdef(TextPosition header, bool res, IList<IDirective> children) : base("ifdef", header, children)
    {
        Result = res;
    }
}