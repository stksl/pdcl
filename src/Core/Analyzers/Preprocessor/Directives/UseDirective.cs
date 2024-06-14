using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;

public sealed class UseDirective : IDirective 
{
    public UseDirective(string asmName, TextPosition pos) : base(asmName, pos)
    {
        
    }
}