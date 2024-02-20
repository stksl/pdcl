using System.Collections.Immutable;
using Pdcl.Core.Diagnostics;
using Pdcl.Core.Syntax;

namespace Pdcl.Core;

/// <summary>
/// Does what as known as Syntax Analysis
/// </summary>
internal sealed partial class Parser 
{
    internal readonly ImmutableList<SyntaxToken> tokens;
    internal readonly DiagnosticHandler diagnostics;

    public readonly SymbolTable GlobalTable;

    internal SyntaxTree? _tree {get; private set;}
    public Parser(ImmutableList<SyntaxToken> _tokens, DiagnosticHandler _handler)
    {
        tokens = _tokens;
        diagnostics = _handler;

        GlobalTable = new SymbolTable();

        _tree = new SyntaxTree(GlobalTable);
    }

    public Task ParseAsync() 
        => VisitorFactory.GetVisitorFor<SyntaxTree.ApplicationContextNode>()!.VisitAsync(this);
}