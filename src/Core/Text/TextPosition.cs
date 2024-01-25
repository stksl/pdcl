using System.Runtime.InteropServices;

namespace Pdcl.Core.Text;

[StructLayout(LayoutKind.Sequential, Size = 8)]
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