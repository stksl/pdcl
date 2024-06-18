using Pdcl.Core.Text;

namespace Pdcl.Core.Syntax;
public readonly struct SyntaxToken 
{
    public static readonly SyntaxToken Missing;
    public readonly SyntaxKind Kind;
    public readonly LexemeMetadata Metadata;

    static SyntaxToken() 
    {
        Missing = new SyntaxToken(SyntaxKind.MissingToken, new LexemeMetadata());
    }
    public SyntaxToken(SyntaxKind kind, LexemeMetadata metadata)
    {
        Kind = kind;
        Metadata = metadata;
    }
}
public readonly struct LexemeMetadata 
{
    public readonly ulong AdditionalMetadata;
    public readonly TextPosition Position;
    public readonly int Line;
    public readonly string Raw;
    public LexemeMetadata(TextPosition pos, int line, string raw, ulong metadata = 0)
    {
        Position = pos;
        Line = line;
        Raw = raw;

        AdditionalMetadata = metadata;
    }
}