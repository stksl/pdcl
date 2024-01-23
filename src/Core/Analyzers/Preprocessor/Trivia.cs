using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;

public readonly struct PreprocTrivia
{
    public readonly TextPosition Position;
    public PreprocTrivia(TextPosition pos)
    {
        Position = pos;
    }
}