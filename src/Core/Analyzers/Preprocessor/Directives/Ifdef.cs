
using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;

public sealed class Ifdef : BranchedDirective 
{
    public Ifdef(TextPosition header, bool res, TextPosition bodyPos) 
        : base("ifdef", header, bodyPos, res)
    {
    }
}