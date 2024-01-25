using System.Collections.Immutable;
using Pdcl.Core.Preproc;
using Pdcl.Core.Syntax;
using Pdcl.Core.Text;

namespace Pdcl.Core;
internal sealed partial class Lexer 
{
    private readonly SourceStream stream;

    private readonly PreprocContext? preprocContext;
    private readonly ImmutableDictionary<int, ISyntaxTrivia>? trivias;
    public Lexer(SourceStream _stream, PreprocContext? context, ImmutableDictionary<int, ISyntaxTrivia>? _trivias)
    {
        stream = _stream;

        preprocContext = context;
        trivias = _trivias;
    }

    public IAnalyzerResult<SyntaxToken, Enum> Next() 
    {
        return default;
    }
}