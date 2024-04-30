using System.Collections.Immutable;
using Pdcl.Core.Diagnostics;
using Pdcl.Core.Syntax;
using System.Collections;
using Pdcl.Core.Assembly;
using System.Runtime.CompilerServices;
using System.Net.Security;
namespace Pdcl.Core;

/// <summary>
/// Does what as known as Syntax Analysis
/// </summary>
internal sealed partial class Parser : IDisposable
{
    public readonly DiagnosticHandler diagnostics;
    public readonly CompilationContext context;
    public readonly AssemblyInfo AssemblyInfo; 
    private readonly Lexer lexer;
    public readonly SyntaxTree tree;

    public volatile string currPath = "/"; 

    public SyntaxToken CurrentToken {get; private set;}
    public bool IsEOF {get; private set;}
    public Parser(Lexer _lexer, DiagnosticHandler _handler, AssemblyInfo assembly)
    {
        lexer = _lexer;

        diagnostics = _handler;
        diagnostics.OnDiagnosticReported += onDiagnosticAsync;

        context = new CompilationContext();

        AssemblyInfo = assembly;

        tree = new SyntaxTree();
    }

    /// <summary>
    /// Consumes one token and returns it skipping trivia
    /// </summary>
    /// <param name="throw_"></param>
    /// <returns></returns>
    /// <exception cref="EndOfStreamException"></exception>
    public SyntaxToken ConsumeToken(bool throwOnEOF = true)
    {
        var result = lexer.Lex();

        if (!result.IsFailed) {
            if (result.Value!.Value.Kind == SyntaxKind.TriviaToken) 
                return ConsumeToken();
            CurrentToken = result.Value!.Value;
        }

        if (result.Status == Lexer.LexerStatusCode.EOF) { 
            IsEOF = true;
            if (throwOnEOF) 
                throw new EndOfStreamException();
        }
        
        return result.Value.GetValueOrDefault();
    }
    /// <summary>
    /// Checks for <paramref name="kind"/> equality and consumes, otherwise missing token
    /// </summary>
    /// <param name="kind"></param>
    /// <returns></returns>
    public SyntaxToken ConsumeToken(SyntaxKind kind, bool throwOnEOF = true) 
    {
        if (CurrentToken.Kind != kind) 
        {
            // just reporting
            diagnostics.ReportUnsuitableSyntaxToken(CurrentToken.Metadata.Line, actual: CurrentToken, expected: kind)
            .Wait();
            return new SyntaxToken(SyntaxKind.MissingToken, ConsumeToken(throwOnEOF).Metadata);
        }
        return ConsumeToken(throwOnEOF);
    }
    public bool IsConsumeMissed(SyntaxKind kind) 
    {
        return ConsumeToken(kind).Kind == SyntaxKind.MissingToken;
    }
    public IEnumerable<SyntaxToken> ConsumeTokens(params SyntaxKind[] kinds) 
    {
        foreach(SyntaxKind kind in kinds) 
        {
            yield return ConsumeToken(kind);
        }
    }
    private async Task onDiagnosticAsync(IDiagnostic diagnostic)
    {
        if (diagnostic is Error error)
            await ErrorRecoverer.RecoverAsync(error, context);
    }
    public Task ParseAsync()
        => VisitorFactory.GetVisitorFor<SyntaxTree.ApplicationContextNode>(context)!.VisitAsync(this);
    public SymbolTreeNode GetCurrentTableNode()
    {
        return AssemblyInfo.TableTree.GetNode(currPath)!;
    }

    public void Dispose()
    {
        /* diagnostics.OnDiagnosticReported -= onDiagnosticAsync; */
    }
}