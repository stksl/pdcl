namespace Pdcl.Core.Text;

public readonly struct TextPosition : IEquatable<TextPosition> 
{
    public readonly int Position;
    public readonly int Length;
    public TextPosition(int pos, int len)
    {
        Position = pos;
        Length = len;
    }
    public bool Equals(TextPosition other) 
    {
        return other.Position == Position && other.Length == Length;
    }
}