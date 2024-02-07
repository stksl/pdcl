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

    public void ReportError(Error err)
    {
        diagnostics.Add(err);
    }
    public void ReportWarning(Warning warn) 
    {
        diagnostics.Add(warn);
    }

    public IEnumerator<IDiagnostic> GetEnumerator() => diagnostics.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => diagnostics.GetEnumerator();
}