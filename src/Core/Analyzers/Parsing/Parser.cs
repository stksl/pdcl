using System.Collections.Immutable;
using Pdcl.Core.Diagnostics;
using Pdcl.Core.Syntax;

namespace Pdcl.Core;

/// <summary>
/// Does what as known as Syntax Analysis
/// </summary>
internal sealed partial class Parser : IDisposable
{
    internal readonly ImmutableList<SyntaxToken> tokens;
    public int tokensInd = 0;
    public SyntaxToken CurrentToken => tokens[tokensInd];
    internal readonly DiagnosticHandler diagnostics;
    internal readonly CompilationContext context;
    public readonly SymbolTable GlobalTable;
    public volatile string currPath = ""; 
    internal SyntaxTree? _tree { get; private set; }
    public Parser(ImmutableList<SyntaxToken> _tokens, DiagnosticHandler _handler)
    {
        tokens = _tokens;
        diagnostics = _handler;
        diagnostics.OnDiagnosticReported += onDiagnostic;
        GlobalTable = new SymbolTable();

        _tree = new SyntaxTree();
        context = new CompilationContext();
    }
    private void onDiagnostic(IDiagnostic diagnostic)
    {
        if (diagnostic is not Error error) return;

        ErrorRecoverer.Recover(error, context);
    }
    public void Dispose()
    {
        diagnostics.OnDiagnosticReported -= onDiagnostic;
    }
    public Task ParseAsync()
        => VisitorFactory.GetVisitorFor<SyntaxTree.ApplicationContextNode>()!.VisitAsync(this);
    public SymbolTable GetCurrentTable()
    {
        throw new NotImplementedException();
    }
}