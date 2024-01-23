using Pdcl.Core.Text;
namespace Pdcl.Core.Preproc;

public abstract class BranchedDirective : IDirective 
{
    public string Name {get; private set;}
    /// <summary>
    /// Header position (for example in #ifdef macro - starting position on # token with length 12) 
    /// </summary>
    public TextPosition Position {get; private set;}
    public BranchedDirective(string name, TextPosition header)
    {
        Name = name;  
        Position = header;
    }
}