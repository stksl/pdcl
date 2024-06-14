
using System.Collections.Immutable;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using Pdcl.Core.Diagnostics;

namespace Pdcl.Core.Syntax;

public abstract class SyntaxNode
{
    public abstract IEnumerable<SyntaxNode> GetChildren();
    internal DiagnosticsBag diagnostics;
    public SyntaxNode()
    {
        diagnostics = new DiagnosticsBag();
    }

    public IEnumerable<IDiagnostic> GetDiagnostics() => diagnostics;
}