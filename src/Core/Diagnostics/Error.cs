using System.Diagnostics.CodeAnalysis;

namespace Pdcl.Core.Diagnostics;

internal struct Error : IDiagnostic
{
    public string Description {get; init;}
    public int Identifier {get; init;}
    public int Line {get; init;}
    public override string ToString()
    {
        return $"ERROR ({Identifier}) at line {Line}: \n\t{Description}";
    }
    public override int GetHashCode() 
    {
        return HashCode.Combine(Identifier, Line);
    }
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Error error && GetHashCode() == error.GetHashCode();
    }
}
public enum ErrorIdentifier : int
{
    UnknownDirectiveDeclaration,
    UnknownMacroCall,

    ArgumentsNotInRange,
    BadTokenError,

    UnsuitableSyntaxToken,
    TerminatorExpected,
    IncorrectNamespaceSyntax,
    UnknownSymbol,
    AlreadyDefined,
    UnkownOperandSyntax,
    NoArgSeparator,
    UnknownOperationSyntax,
    TypeCheckFailure,
    UnkownAccessMods,
}