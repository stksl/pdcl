
namespace Pdcl.Core.Preproc;

public sealed class Ifdef : IDirective 
{
    public string Name {get; private set;}
    public Ifdef(string name)
    {
        Name = name;
    }
}