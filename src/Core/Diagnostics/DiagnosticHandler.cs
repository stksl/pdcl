using System.Collections.Immutable;

namespace Pdcl.Core.Diagnostics;

public interface IDiagnosticHandler
{
    ImmutableList<IDiagnostic> Diagnostics {get;}

}

internal sealed class DiagnosticHandler : IDiagnosticHandler
{
    public ImmutableList<IDiagnostic> Diagnostics => diagnostics.ToImmutableList();
    private DiagnosticsBag diagnostics;
    public DiagnosticHandler()
    {
        diagnostics = new DiagnosticsBag();
    }

    
}