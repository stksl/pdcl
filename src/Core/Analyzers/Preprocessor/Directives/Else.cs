using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;

public sealed class Else : BranchedDirective 
{
    public Else(TextPosition header) : base("else", header)
    {
        
    }
}