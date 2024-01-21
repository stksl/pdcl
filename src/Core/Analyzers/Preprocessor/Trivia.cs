namespace Pdcl.Core.Preproc;

public struct PreprocTrivia
{
    public readonly int Position;
    public readonly int Length;
    public PreprocTrivia(int pos, int len)
    {
        Position = pos;
        Length = len;
    }
}