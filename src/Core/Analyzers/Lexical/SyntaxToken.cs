
using System.Runtime.InteropServices;
using Pdcl.Core.Text;

namespace Pdcl.Core;
public readonly struct SyntaxToken 
{
    public readonly SyntaxKind Kind;
    public readonly LexemeMetadata Metadata;
    public readonly SourceStream Reader;
    public SyntaxToken(SyntaxKind kind, LexemeMetadata metadata, SourceStream reader)
    {
        Kind = kind;
        Metadata = metadata;

        Reader = reader;
    }
}
public readonly struct LexemeMetadata 
{
    public readonly TextPosition Position;
    public readonly int Line;
    public readonly string Raw;
}