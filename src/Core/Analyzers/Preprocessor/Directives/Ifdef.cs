
using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;

public sealed class Ifdef : BranchedDirective 
{
    public readonly bool Result;
    public Ifdef(string name, TextPosition header, bool result) : base(name, header)
    {
        Result = result;
    }
}