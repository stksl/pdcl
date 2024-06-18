using Pdcl.Core.Diagnostics;
using Pdcl.Core.Text;

namespace Pdcl.Core.Syntax;

public interface ISyntaxNodeBuilder 
{

}
internal sealed partial class SyntaxNodeBuilder : ISyntaxNodeBuilder
{
    private readonly DiagnosticHandler _diagnosticHandler;
    private readonly SourceStream _stream;
    public SyntaxNodeBuilder(DiagnosticHandler diagnosticHandler, SourceStream stream)
    {
        _diagnosticHandler = diagnosticHandler;
        _stream = stream;
    }
}