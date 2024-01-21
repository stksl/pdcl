
using Pdcl.Core.Text;

namespace Pdcl.Core;
public struct SyntaxToken 
{
    public SyntaxKind Kind {get; init;}
    public LexemeMetadata Metadata {get; init;}
    public readonly SourceReader Reader;
    public SyntaxToken(SyntaxKind kind, LexemeMetadata metadata, SourceReader reader) : this(reader)
    {
        Kind = kind;
        Metadata = metadata;
    }
    public SyntaxToken(SourceReader reader)
    {
        Reader = reader;
    }
}
public readonly struct LexemeMetadata 
{
    public readonly int Position;
    public readonly int Length;
    public readonly int Line;
    public readonly string Raw;
}