using Pdcl.Core.Text;

namespace Pdcl.Core.Syntax;
public readonly struct SyntaxToken 
{
    public readonly SyntaxKind Kind;
    public readonly LexemeMetadata Metadata;
    public SyntaxToken(SyntaxKind kind, LexemeMetadata metadata)
    {
        Kind = kind;
        Metadata = metadata;
    }
}
public readonly struct LexemeMetadata 
{
    public readonly TextPosition Position;
    public readonly int Line;
    public readonly string Raw;
    public LexemeMetadata(TextPosition pos, int line, string raw)
    {
        Position = pos;
        Line = line;
        Raw = raw;
    }
}