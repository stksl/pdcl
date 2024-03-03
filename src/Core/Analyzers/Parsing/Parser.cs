using System.Collections.Immutable;
using Pdcl.Core.Diagnostics;
using Pdcl.Core.Syntax;
using System.Collections;
namespace Pdcl.Core;

/// <summary>
/// Does what as known as Syntax Analysis
/// </summary>
internal sealed partial class Parser : IDisposable
{
    internal readonly TokenCollection tokens;
    internal readonly DiagnosticHandler diagnostics;
    internal readonly CompilationContext context;
    public readonly SymbolTableTree TableTree;

    public volatile string currPath = "/"; 
    internal SyntaxTree? _tree { get; private set; }
    public Parser(ImmutableList<SyntaxToken> _tokens, DiagnosticHandler _handler)
    {
        tokens = new TokenCollection(_tokens);

        diagnostics = _handler;
        diagnostics.OnDiagnosticReported += onDiagnosticAsync;

        _tree = new SyntaxTree();
        context = new CompilationContext();

        TableTree = new SymbolTableTree();
    }
    private async Task onDiagnosticAsync(IDiagnostic diagnostic)
    {
        if (diagnostic is not Error error) return;
        
        await ErrorRecoverer.RecoverAsync(error, context);
    }
    public void Dispose()
    {
        diagnostics.OnDiagnosticReported -= onDiagnosticAsync;
    }
    public Task ParseAsync()
        => VisitorFactory.GetVisitorFor<SyntaxTree.ApplicationContextNode>()!.VisitAsync(this);
    public SymbolTreeNode GetCurrentTableNode()
    {
        return TableTree.GetNode(currPath)!;
    }
}

public sealed class TokenCollection : IEnumerable<SyntaxToken> 
{
    private readonly ImmutableList<SyntaxToken> tokens;
    public int Index => index;
    private volatile int index;
    public SyntaxToken Current => tokens[index];

    public TokenCollection(ImmutableList<SyntaxToken> tokens_)
    {
        tokens = tokens_;
    }
    public bool SetOffset(int localOffset) => (index += localOffset) >= tokens.Count || index < 0;
    public bool Increment() => SetOffset(1);

    public SyntaxToken? Check(int localOffset) 
    {
        int prev = index;
        SyntaxToken? token = SetOffset(localOffset) ? tokens[index] : null;
        index = prev;
        return token;
    }
    public IEnumerator<SyntaxToken> GetEnumerator() 
    {
        return tokens.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}