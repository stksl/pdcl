using Pdcl.Core.Text;

namespace Pdcl.Core.Preproc;

public abstract class IDirective 
{
    public readonly string Name;
    public readonly TextPosition Position;
    public IDirective(string name, TextPosition pos)
    {
        Name = name;
        Position = pos;
    }
    public override bool Equals(object? obj)
    {
        return obj is IDirective other && other.Position.Position == Position.Position;
    }
    public override int GetHashCode()
    {
        return Position.Position;
    }
}