using System.Collections.Immutable;
using Pdcl.Core.Diagnostics;
using Pdcl.Core.Syntax;
using System.Collections;
using Pdcl.Core.Assembly;
namespace Pdcl.Core;

/// <summary>
/// Does what as known as Syntax Analysis
/// </summary>
internal sealed partial class Parser : IDisposable
{
    public readonly TokenCollection tokens;
    public readonly DiagnosticHandler diagnostics;
    public readonly CompilationContext context;
    public readonly AssemblyInfo AssemblyInfo; 
    public volatile string currPath = "/"; 

    public readonly SyntaxTree tree;
    public Parser(ImmutableList<SyntaxToken> _tokens, DiagnosticHandler _handler, AssemblyInfo assembly)
    {
        tokens = new TokenCollection(_tokens);

        diagnostics = _handler;
        diagnostics.OnDiagnosticReported += onDiagnosticAsync;

        context = new CompilationContext();

        AssemblyInfo = assembly;

        tree = new SyntaxTree();
    }

    private async Task onDiagnosticAsync(IDiagnostic diagnostic)
    {
        if (diagnostic is Error error)
            await ErrorRecoverer.RecoverAsync(error, context);
    }
    public void Dispose()
    {
        diagnostics.OnDiagnosticReported -= onDiagnosticAsync;
    }
    public Task ParseAsync()
        => VisitorFactory.GetVisitorFor<SyntaxTree.ApplicationContextNode>(context)!.VisitAsync(this);
    public SymbolTreeNode GetCurrentTableNode()
    {
        return AssemblyInfo.TableTree.GetNode(currPath)!;
    }
}

public sealed class TokenCollection : IEnumerable<SyntaxToken> 
{
    private readonly ImmutableList<SyntaxToken> tokens;
    public int Index => index;
    private volatile int index;
    public SyntaxToken Current => tokens[index];
    public int Length => tokens.Count;

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