using Pdcl.Core.Text;
namespace Pdcl.Core.Preproc;

public abstract class BranchedDirective : IDirective 
{
    public BranchedDirective(string name, TextPosition header) : base(name, header)
    {
    }
}