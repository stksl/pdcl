using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Pdcl.Core.Syntax;

namespace Pdcl.Core.Diagnostics;

public struct Error : IDiagnostic
{
    public string Description => description;
    private readonly string description;
    public int Identifier => (int)identifier;
    private readonly ErrorIdentifier identifier;
    public int Line => line;
    private readonly int line;
    internal Error(ErrorIdentifier ident, string descr, int line_) 
    {
        description = descr;
        identifier = ident;
        line = line_;
    }
    public override string ToString()
    {
        return $"{identifier} error at line {line}: \n\t{description}";
    }
    public override int GetHashCode() 
    {
        return HashCode.Combine(identifier, line);
    }
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Error error && error.identifier == identifier && error.line == line;
    }
}
public enum ErrorIdentifier : int
{
    #region  preproc
    UnknownDirectiveDeclaration,
    UnknownMacroCall,
    #endregion
    ArgumentsNotInRange,
    BadTokenError,

    UnsuitableSyntaxToken,
    IncorrectNamespaceSyntax,
    UnknownSymbol,
    AlreadyDefined,
    UnkownOperandSyntax,
    NoArgSeparator,
    UnknownOperationSyntax,
    TypeCheckFailure,
    UnkownAccessMods,
}