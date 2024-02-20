using System.Collections.Immutable;
using Pdcl.Core.Syntax;

namespace Pdcl.Core.Diagnostics;

public interface IDiagnosticHandler
{
    ImmutableList<IDiagnostic> Diagnostics {get;}

}

internal sealed class DiagnosticHandler : IDiagnosticHandler
{
    public ImmutableList<IDiagnostic> Diagnostics => diagnostics.ToImmutableList();
    private DiagnosticsBag diagnostics;

    public event DiagnosticDelegate? OnDiagnosticReported;
    public DiagnosticHandler()
    {
        diagnostics = new DiagnosticsBag();
    }

    public void ReportBadToken(int line, SyntaxToken token) 
    {
        reportError(
            new Error(ErrorIdentifier.BadTokenError, $"\'{token.Metadata.Raw}\' was bad token", line));
    }
    public void ReportUnknownDirectiveDeclaration(int line) 
    {
        reportError(
            new Error(ErrorIdentifier.UnknownDirectiveDeclaration, $"\'Unknown directive declaration", line));
    }
    public void ReportArgumentsNotInRange(int line, int argsExpected) 
    {
        reportError(
            new Error(ErrorIdentifier.ArgumentsNotInRange, $"{argsExpected} arguments expected", line));
    }

    private void reportError(Error err) 
    {
        diagnostics.ReportError(err);
        OnDiagnosticReported?.Invoke(err);
    }
}