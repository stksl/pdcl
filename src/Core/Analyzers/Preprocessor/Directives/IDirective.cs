using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;

public interface IDirective 
{
    string Name {get;}

    TextPosition Position {get;}
}