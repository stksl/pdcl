using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;

public sealed class IfNotdef : BranchedDirective 
{
    public IfNotdef(TextPosition header, bool res, TextPosition bodyPos) 
        : base("ifndef", header, bodyPos, res)
    {
    }
}