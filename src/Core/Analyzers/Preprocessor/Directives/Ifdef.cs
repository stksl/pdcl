
using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;

public sealed class Ifdef : BranchedDirective 
{
    public Ifdef(TextPosition header, bool res, IList<IDirective> children, TextPosition bodyPos) 
        : base("ifdef", header, children, bodyPos)
    {
        Result = res;
    }
}