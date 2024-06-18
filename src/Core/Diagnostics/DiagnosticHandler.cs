using System.Collections.Immutable;
using Pdcl.Core.Syntax;

namespace Pdcl.Core.Diagnostics;

public interface IDiagnosticHandler
{
    ImmutableList<IDiagnostic> Diagnostics { get; }
    event DiagnosticDelegate? OnDiagnosticReported;
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
        reportError(new Error()
        {
            Identifier = (int)ErrorIdentifier.BadTokenError,
            Description = $"\'{token.Metadata.Raw}\' was bad token",
            Line = line
        });
    }
    public void ReportUnknownDirectiveDeclaration(int line)
    {
        reportError(new Error()
        {
            Identifier = (int)ErrorIdentifier.UnknownDirectiveDeclaration,
            Description = $"\'Unknown directive declaration",
            Line = line
        });
    }
    public void ReportArgumentsNotInRange(int line, int argsExpected)
    {
        reportError(new Error()
        {
            Identifier = (int)ErrorIdentifier.ArgumentsNotInRange,
            Description = $"{argsExpected} arguments expected",
            Line = line
        });
    }
    public void ReportUnsuitableSyntaxToken(int line, SyntaxKind actual, SyntaxKind expected)
    {
        reportError(new Error()
        {
            Identifier = (int)ErrorIdentifier.UnsuitableSyntaxToken,
            Description = $"Expected {expected}, instead got \'{actual}\'",
            Line = line
        });
    }
    public void ReportTerminatorExpected(int line, SyntaxKind actual, SyntaxKind expected)
    {
        reportError(new Error()
        {
            Identifier = (int)ErrorIdentifier.TerminatorExpected,
            Description = $"Expected {expected} terminator, instead got \'{actual}\'",
            Line = line
        });
    }
    public void ReportIncorrectNamespaceSyntax(int line, string namespaceName)
    {
        reportError(new Error() { Identifier = (int)ErrorIdentifier.IncorrectNamespaceSyntax, Description = $"Incorrect namespace identifier syntax: {namespaceName}", Line = line });
    }
    public void ReportUnknownSymbol(int line, string symbolName)
    {
        reportError(new Error() { Identifier = (int)ErrorIdentifier.UnknownSymbol, Description = $"Unknown symbol: {symbolName}", Line = line });
    }
    public void ReportAlreadyDefined(int line, string name)
    {
        reportError(new Error() { Identifier = (int)ErrorIdentifier.AlreadyDefined, Description = $"\"{name}\" has already been defined", Line = line });
    }
    public void ReportUnkownOperandSyntax(int line, string operand)
    {
        reportError(new Error() { Identifier = (int)ErrorIdentifier.UnkownOperandSyntax, Description = $"Unknown operand syntax: {operand}", Line = line });
    }
    public void ReportUnkownOperationSyntax(int line, string operation)
    {
        reportError(new Error() { Identifier = (int)ErrorIdentifier.UnknownOperationSyntax, Description = $"Unknown operation syntax: {operation}", Line = line });
    }
    public void ReportNoArgSeparator(int line)
    {
        reportError(new Error() { Identifier = (int)ErrorIdentifier.NoArgSeparator, Description = $"No argument separator", Line = line });
    }
    public void ReportTypeCheck(int line)
    {
        reportError(new Error() { Identifier = (int)ErrorIdentifier.TypeCheckFailure, Description = $"Type check failure", Line = line });
    }
    public void ReportUnkownAccessMods(int line)
    {
        reportError(new Error() { Identifier = (int)ErrorIdentifier.UnkownAccessMods, Description = $"Not a correct access modifier", Line = line });
    }
    private void reportError(Error err)
    {
        diagnostics.ReportError(err);
        OnDiagnosticReported!.Invoke(err);
    }
}