using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;

public sealed class IfNotdef : BranchedDirective 
{
    public IfNotdef(TextPosition header, IList<IDirective> children) : base("ifndef", header, children)
    {
        
    }
}