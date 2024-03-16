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

    public Task ReportBadToken(int line, SyntaxToken token) 
    {
        return reportErrorAsync(
            new Error(ErrorIdentifier.BadTokenError, $"\'{token.Metadata.Raw}\' was bad token", line));
    }
    public Task ReportUnknownDirectiveDeclaration(int line) 
    {
        return reportErrorAsync(
            new Error(ErrorIdentifier.UnknownDirectiveDeclaration, $"\'Unknown directive declaration", line));
    }
    public Task ReportArgumentsNotInRange(int line, int argsExpected) 
    {
        return reportErrorAsync(
            new Error(ErrorIdentifier.ArgumentsNotInRange, $"{argsExpected} arguments expected", line));
    }
    public Task ReportUnsuitableSyntaxToken(int line, SyntaxToken actual, SyntaxKind expected) 
    {
        return reportErrorAsync(new Error(
            ErrorIdentifier.UnsuitableSyntaxToken, $"Expected {expected}, instead got {actual.Metadata.Raw}({actual.Kind})", line
        ));
    }
    public Task ReportIncorrectNamespaceSyntax(int line, string namespaceName) 
    {
        return reportErrorAsync(new Error(ErrorIdentifier.IncorrectNamespaceSyntax, $"Incorrect namespace identifier syntax: {namespaceName}", line));
    }
    public Task ReportSemicolonExpected(int line) 
    {
        return reportErrorAsync(new Error(ErrorIdentifier.SemicolonExpected, "Semicolon expected", line));
    }
    public Task ReportUnknownSymbol(int line, string symbolName) 
    {
        return reportErrorAsync(new Error(ErrorIdentifier.UnknownSymbol, $"Unknown symbol: {symbolName}", line));
    }
    public Task ReportAlreadyDefined(int line, string name) 
    {
        return reportErrorAsync(new Error(ErrorIdentifier.AlreadyDefined, $"{name} has already been defined", line));
    }
    public Task ReportUnkownOperandSyntax(int line, string operand) 
    {
        return reportErrorAsync(new Error(ErrorIdentifier.UnkownOperandSyntax, $"Unknown operand syntax: {operand}", line));
    }
    public Task ReportUnkownOperationSyntax(int line, string operation) 
    {
        return reportErrorAsync(new Error(ErrorIdentifier.UnknownOperationSyntax, $"Unknown operation syntax: {operation}", line));
    }
    public Task ReportNoArgSeparator(int line) 
    {
        return reportErrorAsync(new Error(ErrorIdentifier.NoArgSeparator, $"No argument separator", line));
    }
    public Task ReportTypeCheck(int line) 
    {
        return reportErrorAsync(new Error(ErrorIdentifier.TypeCheckFailure, $"Type check failure", line));
    }
    private async Task reportErrorAsync(Error err) 
    {
        diagnostics.ReportError(err);
        await OnDiagnosticReported!.Invoke(err);
    }
}