using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;

public sealed class EndIf : BranchedDirective 
{
    public EndIf(TextPosition pos) : base("endif", pos)
    {
        
    }
}