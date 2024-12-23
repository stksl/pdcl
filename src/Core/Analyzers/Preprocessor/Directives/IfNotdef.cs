using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;

public sealed class IfNotdef : BranchedDirective 
{
    public IfNotdef(TextPosition header, bool res, IList<IDirective> children, TextPosition bodyPos) 
        : base("ifndef", header, children, bodyPos)
    {
        Result = res;
    }
}