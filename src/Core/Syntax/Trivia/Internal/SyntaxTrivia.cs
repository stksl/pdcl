using Pdcl.Core.Text;
namespace Pdcl.Core.Syntax;
internal sealed class SyntaxTrivia : ISyntaxTrivia
{
    public TextPosition Position {get; private set;}
    public SyntaxTrivia(TextPosition pos)
    {
        Position = pos;
    }
}