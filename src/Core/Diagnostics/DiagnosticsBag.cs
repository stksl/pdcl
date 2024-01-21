using System.CodeDom.Compiler;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Pdcl.Core.Diagnostics;

public sealed class DiagnosticsBag : IEnumerable<IDiagnostic>
{
    private List<IDiagnostic> diagnostics;
    public int Count => diagnostics.Count;
    public DiagnosticsBag()
    {
        diagnostics = new List<IDiagnostic>();
    }

    public void Add(IDiagnostic item)
    {
        diagnostics.Add(item);
    }

    public IEnumerator<IDiagnostic> GetEnumerator() => diagnostics.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => diagnostics.GetEnumerator();
}